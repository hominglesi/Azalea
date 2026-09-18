using Azalea.Native.OpenAL;
using System.Diagnostics;

namespace Azalea.Platform.Audio.OpenAL;
internal class ALAudio : PlatformAudio
{
	private readonly IAudioDeviceNotificationClient _deviceNotificationClient;
	private static readonly int[] _contextAttributes = [ALC.HRTF_SOFT, ALC.FALSE, 0];

	private const int _audioChannelCount = 4;
	private const int _audioByteChannelInternalCount = 4;

	private nint _device;
	private nint _context;

	private uint[] _audioSources = new uint[_audioChannelCount];

	private uint[] _audioByteSourcesInternal = new uint[_audioByteChannelInternalCount];

	internal ALAudio(IAudioDeviceNotificationClient deviceNotificationClient)
	{
		// We need to keep a reference to the client alive
		_deviceNotificationClient = deviceNotificationClient;

		Thread.Enqueue(InitializeCommand.Borrow())!.Await();

		Debug.Assert(_device != nint.Zero);
		Debug.Assert(_context != nint.Zero);
	}

	private int _currentAudioSource = 0;

	protected override void UpdateLogic()
	{
		for (int i = 0; i < ActiveInstances.Count; i++)
		{
			var audioInstance = ActiveInstances[i];
			if (audioInstance is AudioByteInstance byteInstance)
			{
				int sourceState = 0;
				Debug.Assert(byteInstance.Source is not null);
				AL.GetSourcei(byteInstance.Source.Handle, AL.SOURCE_STATE, ref sourceState);

				float sourceOffset = 0;
				AL.GetSourcef(byteInstance.Source.Handle, AL.SEC_OFFSET, ref sourceOffset);
				byteInstance.Timestamp = sourceOffset;

				if (sourceState == AL.STOPPED)
				{
					markInstanceStopped(audioInstance);
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

				AL.GenSources(_audioSources.Length, ref _audioSources[0]);

				var byteSourceHandles = new uint[AudioByteSources.Length];
				AL.GenSources(AudioByteSources.Length, ref byteSourceHandles[0]);
				for (int i = 0; i < AudioByteSources.Length; i++)
					AudioByteSources[i] = new ALByteSource(byteSourceHandles[i]);

				AL.GenSources(_audioByteSourcesInternal.Length, ref _audioByteSourcesInternal[0]);

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
				}

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

				audioByteSource.CurrentInstance = instance;

				instance.Source = audioByteSource;
				instance.State = AudioInstanceState.Playing;

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
				}

				break;
			case SetInstanceTimestampCommand(var instance, var timestamp):
				switch (instance)
				{
					case AudioByteInstance byteInstance:
						Debug.Assert(byteInstance.Source is not null);
						AL.Sourcef(byteInstance.Source.Handle, AL.SEC_OFFSET, timestamp);
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
				}
				
				break;
		}

		command.Return();
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

		if (instance is AudioByteInstance byteInstance)
		{
			Debug.Assert(byteInstance.Source is not null);
			byteInstance.Source.CurrentInstance = null;

			byteInstance.InvokeStopped();

			byteInstance.State = AudioInstanceState.Stopped;
		}

		ActiveInstances.Remove(instance);
	}
}
