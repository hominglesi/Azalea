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
			case PlayByteCommand(var instance, var sound, var gain, var looping):
				Debug.Assert(sound.Handle is not null);

				var audioByteSource = getNextByteSource();

				if (audioByteSource.CurrentInstance.Value is not null)
				{
					AL.SourceStop(audioByteSource.Handle);
					markInstanceStopped(audioByteSource.CurrentInstance.Value);
				}

				AL.Sourcei(audioByteSource.Handle, AL.BUFFER, (int)sound.Handle);
				AL.Sourcef(audioByteSource.Handle, AL.GAIN, gain);
				AL.Sourcei(audioByteSource.Handle, AL.LOOPING, looping ? 1 : 0);

				AL.SourcePlay(audioByteSource.Handle);

				audioByteSource.CurrentInstance.Value = instance;

				instance.Source = audioByteSource;
				instance.State = AudioInstanceState.Playing;

				ActiveInstances.Add(instance);
				InstanceStarted?.Invoke(instance);
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
		if (instance is AudioByteInstance byteInstance)
		{
			Debug.Assert(byteInstance.Source is not null);
			byteInstance.Source.CurrentInstance.Value = null;

			byteInstance.InvokeStopped();
		}

		ActiveInstances.Remove(instance);
	}
}
