using Azalea.Native.OpenAL;
using Azalea.Sounds;
using System;
using System.Diagnostics;

namespace Azalea.Platform.Audio.OpenAL;
internal class ALAudio : PlatformAudio
{
	private readonly IAudioDeviceNotificationClient _deviceNotificationClient;
	private static readonly int[] _contextAttributes = [ALC.HRTF_SOFT, ALC.FALSE, 0];

	private const int _audioChannelCount = 4;
	private const int _audioByteChannelCount = 24;
	private const int _audioByteChannelInternalCount = 4;

	private nint _device;
	private nint _context;

	private ALAudioSource[] _audioSources = new ALAudioSource[_audioChannelCount];
	private ALAudioByteSource[] _audioByteSources = new ALAudioByteSource[_audioByteChannelCount];
	private ALAudioByteSource[] _audioByteSourcesInternal = new ALAudioByteSource[_audioByteChannelInternalCount];

	internal ALAudio(IAudioDeviceNotificationClient deviceNotificationClient)
	{
		// We need to keep a reference to the client alive
		_deviceNotificationClient = deviceNotificationClient;

		Thread.Enqueue(InitializeCommand.Borrow())!.Await();

		Debug.Assert(_device != nint.Zero);
		Debug.Assert(_context != nint.Zero);
	}

	private int _currentAudioSource = 0;
	private int _currentAudioByteSource = 0;

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

				for (int i = 0; i < _audioSources.Length; i++)
					_audioSources[i] = new ALAudioSource();
				for (int i = 0; i < _audioByteSources.Length; i++)
					_audioByteSources[i] = new ALAudioByteSource();
				for (int i = 0; i < _audioByteSourcesInternal.Length; i++)
					_audioByteSourcesInternal[i] = new ALAudioByteSource();
				break;

			case BindSourceBufferCommand(var source, var buffer):
				AL.Sourcei(source, AL.BUFFER, (int)buffer);
				break;
			case BufferDataCommand(var buffer, byte[] data, int dataLength, int format, int frequency, bool freeData):
				AL.BufferData(buffer, format, ref data[0], dataLength, frequency);
				break;
			case CloseDeviceCommand(var device):
				ALC.CloseDevice(device);
				break;
			case CreateSoundByteCommand(var soundByte, byte[] data, int dataLength, int format, int frequency):
				uint bufferHandle = 0;
				AL.GenBuffers(1, ref bufferHandle);
				AL.BufferData(bufferHandle, format, ref data[0], dataLength, frequency);

				var channels = format switch
				{
					AL.FORMAT_MONO8 => 1,
					AL.FORMAT_MONO16 => 1,
					AL.FORMAT_STEREO8 => 2,
					AL.FORMAT_STEREO16 => 2,
					_ => throw new NotImplementedException()
				};

				var bits = format switch
				{
					AL.FORMAT_MONO8 => 8,
					AL.FORMAT_MONO16 => 16,
					AL.FORMAT_STEREO8 => 8,
					AL.FORMAT_STEREO16 => 16,
					_ => throw new NotImplementedException()
				};

				var duration = dataLength / (channels * (bits / 8) * frequency);

				soundByte.Initialize(bufferHandle, duration);
				break;
			case PlayCommand(var sound, var gain, var looping):
				var audioSource = _audioSources[_currentAudioSource];
				_currentAudioSource = (_currentAudioSource + 1) % _audioSources.Length;

				if (audioSource.State == AudioSourceState.Playing || audioSource.State == AudioSourceState.Paused)
					audioSource.Stop();

				audioSource.Play(sound, gain, looping);

				break;
			case PlayByteCommand(var sound, var gain, var looping):
				var audioByteSource = _audioByteSources[_currentAudioSource];
				_currentAudioByteSource = (_currentAudioByteSource + 1) % _audioByteSources.Length;

				Debug.Assert(sound.Handle is not null);
				AL.Sourcei(audioByteSource.Handle, AL.BUFFER, (int)sound.Handle);

				AL.Sourcef(audioByteSource.Handle, AL.GAIN, gain);
				AL.Sourcei(audioByteSource.Handle, AL.LOOPING, looping ? 1 : 0);

				AL.SourcePlay(audioByteSource.Handle);

				break;
			case SetMasterVolumeCommand(var volume):
				AL.Listenerf(AL.GAIN, volume);
				MasterVolume.Value = volume;
				break;
		}

		command.Return();
	}
}
