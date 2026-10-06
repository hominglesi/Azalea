using Azalea.Native.OpenAL;
using Azalea.Sounds.FFmpeg;
using System.Buffers;
using System.Diagnostics;

namespace Azalea.Platform.Audio.OpenAL;

internal class ALAudio : PlatformAudio
{
	private readonly IAudioDeviceNotificationClient _deviceNotificationClient;
	private static readonly int[] _contextAttributes = [ALC.HRTF_SOFT, ALC.FALSE, 0];

	private nint _device;
	private nint _context;

	internal ALAudio(IAudioDeviceNotificationClient deviceNotificationClient)
	{
		// We need to keep a reference to the client alive
		_deviceNotificationClient = deviceNotificationClient;

		Thread.Enqueue(InitializeCommand.Borrow())!.Await();

		Debug.Assert(_device != nint.Zero);
		Debug.Assert(_context != nint.Zero);
	}

	protected override void UpdateLogic()
	{
		for (int i = 0; i < ActiveInstances.Count; i++)
		{
			var instance = ActiveInstances[i];
			if (instance is AudioInstance audioInstance)
			{
				if (audioInstance.State != AudioInstanceState.Playing)
					continue;

				Debug.Assert(audioInstance.Source is not null);
				var source = audioInstance.Source;

				Debug.Assert(source.CurrentReader is not null);

				int buffersProcessed = 0;
				AL.GetSourcei(source.Handle, AL.BUFFERS_PROCESSED, ref buffersProcessed);
				while (buffersProcessed-- > 0)
				{
					uint buffer = 0;
					AL.SourceUnqueueBuffers(source.Handle, 1, ref buffer);
					source.CurrentBufferStartTime = (source.CurrentBufferStartTime + 1) % source.Buffers.Length;

					if (source.CurrentReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
					{
						AL.BufferData(buffer, AL.FORMAT_STEREO16, ref pcm[0], pcmLength, sampleRate);
						ArrayPool<byte>.Shared.Return(pcm);
						AL.SourceQueueBuffers(source.Handle, 1, ref buffer);
						source.BufferStartTimes[source.NextBufferStartTime] = startTime;
					}

					source.NextBufferStartTime = (source.NextBufferStartTime + 1) % source.Buffers.Length;
				}

				int sourceState = 0;
				AL.GetSourcei(source.Handle, AL.SOURCE_STATE, ref sourceState);

				if (sourceState != AL.PLAYING)
					AL.SourcePlay(source.Handle);

				AL.GetSourcei(source.Handle, AL.SOURCE_STATE, ref sourceState);
				if (sourceState == AL.STOPPED)
				{
					if (audioInstance.Looping == false)
					{
						audioInstance.State = AudioInstanceState.Paused;
					}
					else
					{
						source.CurrentReader.Seek(0);
						for (int j = 0; j < source.Buffers.Length; j++)
						{
							if (source.CurrentReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
							{
								AL.BufferData(source.Buffers[i], AL.FORMAT_STEREO16, ref pcm[0], pcmLength, sampleRate);
								ArrayPool<byte>.Shared.Return(pcm);
								source.BufferStartTimes[i] = startTime;
							}
						}

						source.CurrentBufferStartTime = 0;
						source.NextBufferStartTime = 0;
						AL.SourceQueueBuffers(source.Handle, source.Buffers.Length, ref source.Buffers[0]);
					}
				}

				AL.GetSourcef(source.Handle, AL.SEC_OFFSET, ref source.SourceOffset);
				audioInstance.Timestamp = source.BufferStartTimes[source.CurrentBufferStartTime] + source.SourceOffset;
			}
			else if (instance is AudioByteInstance byteInstance)
			{
				Debug.Assert(byteInstance.Source is not null);

				int sourceState = 0;
				AL.GetSourcei(byteInstance.Source.Handle, AL.SOURCE_STATE, ref sourceState);

				float sourceOffset = 0;
				AL.GetSourcef(byteInstance.Source.Handle, AL.SEC_OFFSET, ref sourceOffset);
				byteInstance.Timestamp = sourceOffset;

				if (sourceState == AL.STOPPED)
				{
					markInstanceStopped(byteInstance);
					i--;
				}
			}
		}
	}

	protected override void HandleCommandLogic(AudioCommand command)
	{
		switch (command)
		{
			case InitializeCommand():
				_device = ALC.OpenDevice(null);

				if (ALC.DynamicFunctionsLoaded == false)
					ALC.LoadDynamicFunctions(ALC.GetProcAddress, _device);
				_context = ALC.CreateContext(_device, ref _contextAttributes[0]);
				ALC.MakeContextCurrent(_context);

				_deviceNotificationClient.DefaultDeviceChanged += () => ALC.ReopenDeviceSOFT(_device, null, ref _contextAttributes[0]);

				AL.DistanceModel(0);
				AL.Listener3f(AL.POSITION, 0, 0, 0);
				AL.Listener3f(AL.VELOCITY, 0, 0, 0);

				var deviceFrequency = 0;
				ALC.GetIntegerv(_device, ALC.FREQUENCY, 1, ref deviceFrequency);
				FFmpegStreamReader.FREQUENCY = deviceFrequency;

				var sourceHandles = new uint[AudioSources.Length];
				AL.GenSources(AudioSources.Length, ref sourceHandles[0]);
				for (int i = 0; i < AudioSources.Length; i++)
				{
					var sourceBuffers = new uint[ALSource.BufferCount];
					AL.GenBuffers(ALSource.BufferCount, ref sourceBuffers[0]);
					AudioSources[i] = new ALSource(sourceHandles[i], sourceBuffers);
				}

				var byteSourceHandles = new uint[AudioByteSources.Length];
				AL.GenSources(AudioByteSources.Length, ref byteSourceHandles[0]);
				for (int i = 0; i < AudioByteSources.Length; i++)
					AudioByteSources[i] = new ALByteSource(byteSourceHandles[i]);

				break;
			case CreateSoundByteCommand(var soundByte, byte[] data, int dataLength, int format, int frequency):
				uint bufferHandle = 0;
				AL.GenBuffers(1, ref bufferHandle);
				AL.BufferData(bufferHandle, format, ref data[0], dataLength, frequency);

				soundByte.Initialize(bufferHandle);
				break;
			case PauseInstanceCommand(var instance):
				switch (instance)
				{
					case AudioByteInstance byteInstance:
						Debug.Assert(byteInstance.Source is not null);
						AL.SourcePause(byteInstance.Source.Handle);
						byteInstance.State = AudioInstanceState.Paused;
						break;
					case AudioInstance audioInstance:
						Debug.Assert(audioInstance.Source is not null);
						AL.SourcePause(audioInstance.Source.Handle);
						audioInstance.State = AudioInstanceState.Paused;
						break;
				}

				break;
			case PlayCommand(var instance, var sound):
				var audioSource = getNextSource();

				if (audioSource.CurrentInstance is not null)
				{
					AL.SourceStop(audioSource.Handle);
					markInstanceStopped(audioSource.CurrentInstance);
				}

				var stream = sound.GetStream();
				Debug.Assert(stream is not null);

				var streamReader = new FFmpegStreamReader(stream);
				audioSource.CurrentReader = streamReader;
				instance.Duration = streamReader.TotalDuration;

				for (int i = 0; i < audioSource.Buffers.Length; i++)
				{
					if (streamReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
					{
						AL.BufferData(audioSource.Buffers[i], AL.FORMAT_STEREO16, ref pcm[0], pcmLength, sampleRate);
						ArrayPool<byte>.Shared.Return(pcm);
						audioSource.BufferStartTimes[i] = startTime;
					}
				}

				audioSource.CurrentBufferStartTime = 0;
				audioSource.NextBufferStartTime = 0;

				AL.SourceQueueBuffers(audioSource.Handle, audioSource.Buffers.Length, ref audioSource.Buffers[0]);
				AL.Sourcef(audioSource.Handle, AL.GAIN, instance.Gain);
				AL.Sourcei(audioSource.Handle, AL.LOOPING, instance.Looping ? 1 : 0);

				AL.SourcePlay(audioSource.Handle);

				instance.Source = audioSource;
				instance.State = AudioInstanceState.Playing;

				audioSource.CurrentInstance = instance;

				ActiveInstances.Add(instance);
				InstanceStarted?.Invoke(instance);
				break;
			case PlayByteCommand(var instance, var sound):
				Debug.Assert(sound.Handle is not null);

				var audioByteSource = getNextByteSource();

				if (audioByteSource.CurrentInstance is not null)
				{
					AL.SourceStop(audioByteSource.Handle);
					markInstanceStopped(audioByteSource.CurrentInstance);
				}

				AL.Sourcei(audioByteSource.Handle, AL.BUFFER, (int)sound.Handle);
				AL.Sourcef(audioByteSource.Handle, AL.GAIN, instance.Gain);
				AL.Sourcei(audioByteSource.Handle, AL.LOOPING, instance.Looping ? 1 : 0);

				AL.SourcePlay(audioByteSource.Handle);

				instance.Source = audioByteSource;
				instance.State = AudioInstanceState.Playing;

				audioByteSource.CurrentInstance = instance;

				ActiveInstances.Add(instance);
				InstanceStarted?.Invoke(instance);
				break;
			case SetInstanceGainCommand(var instance, var gain):
				switch (instance)
				{
					case AudioByteInstance byteInstance:
						Debug.Assert(byteInstance.Source is not null);
						AL.Sourcef(byteInstance.Source.Handle, AL.GAIN, gain);
						byteInstance.Gain = gain;
						break;
					case AudioInstance audioInstance:
						Debug.Assert(audioInstance.Source is not null);
						AL.Sourcef(audioInstance.Source.Handle, AL.GAIN, gain);
						audioInstance.Gain = gain;
						break;
				}

				break;
			case SetInstanceTimestampCommand(var instance, var timestamp):
				switch (instance)
				{
					case AudioByteInstance byteInstance:
						Debug.Assert(byteInstance.Source is not null);
						AL.Sourcef(byteInstance.Source.Handle, AL.SEC_OFFSET, timestamp);
						break;
					case AudioInstance audioInstance:
						Debug.Assert(audioInstance.Source is not null);
						Debug.Assert(audioInstance.Source.CurrentReader is not null);

						AL.SourceStop(audioInstance.Source.Handle);
						var buffersQueued = 0;
						AL.GetSourcei(audioInstance.Source.Handle, AL.BUFFERS_QUEUED, ref buffersQueued);
						if (buffersQueued > 0)
						{
							var unqueued = new uint[buffersQueued];
							AL.SourceUnqueueBuffers(audioInstance.Source.Handle, unqueued.Length, ref unqueued[0]);
						}

						audioInstance.Source.SourceOffset = 0;

						audioInstance.Source.CurrentReader.Seek(timestamp);
						for (int i = 0; i < audioInstance.Source.Buffers.Length; i++)
						{
							if (audioInstance.Source.CurrentReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
							{
								AL.BufferData(audioInstance.Source.Buffers[i], AL.FORMAT_STEREO16, ref pcm[0], pcmLength, sampleRate);
								ArrayPool<byte>.Shared.Return(pcm);
								audioInstance.Source.BufferStartTimes[i] = startTime;
							}
						}

						audioInstance.Source.CurrentBufferStartTime = 0;
						audioInstance.Source.NextBufferStartTime = 0;
						AL.SourceQueueBuffers(audioInstance.Source.Handle, audioInstance.Source.Buffers.Length, ref audioInstance.Source.Buffers[0]);

						if (audioInstance.State == AudioInstanceState.Playing)
							AL.SourcePlay(audioInstance.Source.Handle);
						break;
				}

				break;
			case StopInstanceCommand(var instance):
				switch (instance)
				{
					case AudioByteInstance byteInstance:
						Debug.Assert(byteInstance.Source is not null);
						AL.SourceStop(byteInstance.Source.Handle);
						break;
					case AudioInstance audioInstance:
						Debug.Assert(audioInstance.Source is not null);
						AL.SourceStop(audioInstance.Source.Handle);

						var buffersQueued = 0;
						AL.GetSourcei(audioInstance.Source.Handle, AL.BUFFERS_QUEUED, ref buffersQueued);
						if (buffersQueued > 0)
						{
							var unqueued = new uint[buffersQueued];
							AL.SourceUnqueueBuffers(audioInstance.Source.Handle, unqueued.Length, ref unqueued[0]);
						}

						audioInstance.Source.CurrentReader?.Dispose();
						audioInstance.Source.CurrentReader = null;

						break;
				}

				markInstanceStopped(instance);
				break;
			case UnpauseInstanceCommand(var instance):
				if (instance.State != AudioInstanceState.Paused)
					break;

				switch (instance)
				{
					case AudioByteInstance byteInstance:
						Debug.Assert(byteInstance.Source is not null);
						AL.SourcePlay(byteInstance.Source.Handle);
						byteInstance.State = AudioInstanceState.Playing;
						break;
					case AudioInstance audioInstance:
						Debug.Assert(audioInstance.Source is not null);
						AL.SourcePlay(audioInstance.Source.Handle);
						audioInstance.State = AudioInstanceState.Playing;
						break;
				}

				break;
		}

		command.Return();
	}

	private const int _audioChannelCount = 8;
	internal ALSource[] AudioSources = new ALSource[_audioChannelCount];
	private int _nextAudioSource = 0;
	private ALSource getNextSource()
	{
		var next = AudioSources[_nextAudioSource];
		_nextAudioSource = (_nextAudioSource + 1) % AudioSources.Length;
		return next;
	}

	private const int _audioByteChannelCount = 24;
	internal ALByteSource[] AudioByteSources = new ALByteSource[_audioByteChannelCount];
	private int _nextAudioByteSource = 0;
	private ALByteSource getNextByteSource()
	{
		var next = AudioByteSources[_nextAudioByteSource];
		_nextAudioByteSource = (_nextAudioByteSource + 1) % AudioByteSources.Length;
		return next;
	}

	private void markInstanceStopped(IAudioInstance instance)
	{
		Debug.Assert(instance.State != AudioInstanceState.Stopped);

		if (instance is AudioInstance audioInstance)
		{
			Debug.Assert(audioInstance.Source is not null);
			audioInstance.Source.CurrentInstance = null;
			audioInstance.InvokeStopped();
			audioInstance.State = AudioInstanceState.Stopped;
		}
		else if (instance is AudioByteInstance byteInstance)
		{
			Debug.Assert(byteInstance.Source is not null);
			byteInstance.Source.CurrentInstance = null;
			byteInstance.InvokeStopped();
			byteInstance.State = AudioInstanceState.Stopped;
		}

		ActiveInstances.Remove(instance);
	}
}
