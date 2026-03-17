using Azalea.Platform;
using System;
using System.Numerics;

namespace Azalea.Sounds.OpenAL;
internal partial class ALAudioManager : AudioManager
{
	// These should add up to 32
	protected const int AudioSourceCount = 4;
	protected const int AudioByteSourceCount = 24;
	protected const int AudioByteSourceInternalCount = 4;

	private readonly IAudioDeviceNotificationClient _deviceNotificationClient;

	private ALDevice _device;
	private readonly IntPtr _context;

	public static int DEVICE_FREQUENCY;

	private readonly ALAudioSource[] _audioSources;
	private readonly ALAudioByteSource[] _audioByteSources;
	private readonly ALAudioByteSource[] _audioByteSourcesInternal;

	public override IAudioSource[] AudioChannels => _audioSources;
	public override IAudioSource[] AudioByteChannels => _audioByteSources;
	public override IAudioSource[] AudioByteInternalChannels => _audioByteSourcesInternal;

	private readonly int[] _deviceAttributes = [0x1992 /* ALC_HRTF_SOFT */, 0 /* ALC_FALSE */, 0];

	public ALAudioManager(IAudioDeviceNotificationClient deviceNotificationClient)
	{
		// We need to keep a reference to the client alive
		_deviceNotificationClient = deviceNotificationClient;

		_device = new ALDevice(this, OpenDevice(null));
		_context = CreateContext(_device.Handle, _deviceAttributes);
		MakeContextCurrent(_context);

		_deviceNotificationClient.DefaultDeviceChanged += () => _device.Reopen(null, _deviceAttributes);

		SetDistanceModel(0);
		SetListenerPosition(Vector3.Zero);
		SetListenerVelocity(Vector3.Zero);

		DEVICE_FREQUENCY = _device.GetFrequency();

		_audioSources = new ALAudioSource[AudioSourceCount];
		for (int i = 0; i < AudioSourceCount; i++)
			_audioSources[i] = new ALAudioSource(this);

		_audioByteSources = new ALAudioByteSource[AudioByteSourceCount];
		for (int i = 0; i < AudioByteSourceCount; i++)
			_audioByteSources[i] = new ALAudioByteSource(this);

		_audioByteSourcesInternal = new ALAudioByteSource[AudioByteSourceInternalCount];
		for (int i = 0; i < AudioByteSourceInternalCount; i++)
			_audioByteSourcesInternal[i] = new ALAudioByteSource(this);
	}

	private float _masterVolume = 1.0f;
	public override float MasterVolume
	{
		get => _masterVolume;
		set
		{
			if (_masterVolume == value) return;

			IssueCommand(new SetListenerGainCommand(_masterVolume));

			_masterVolume = value;
		}
	}

	public override SoundByte CreateSoundByte(byte[] data, int dataLength, ALFormat format, int frequency)
		=> new ALSound(this, data, dataLength, format, frequency);

	private int _currentAudioSource = 0;
	public override IAudioInstance Play(Sound sound, float gain = 1, bool looping = false)
	{
		var audioSource = _audioSources[_currentAudioSource];

		_currentAudioSource = (_currentAudioSource + 1) % AudioSourceCount;

		return audioSource.Play(sound, gain, looping)!;
	}

	private int _currentAudioByteSource = 0;
	public override IAudioInstance PlayByte(SoundByte soundByte, float gain = 1, bool looping = false)
	{
		var audioByteSource = _audioByteSources[_currentAudioByteSource];

		_currentAudioByteSource = (_currentAudioByteSource + 1) % AudioByteSourceCount;

		return audioByteSource.Play(soundByte, gain, looping)!;
	}

	private int _currentAudioByteSourceInternal = 0;
	public override IAudioInstance PlayByteInternal(SoundByte soundByte, float gain = 1, bool looping = false)
	{
		var audioByteSource = _audioByteSourcesInternal[_currentAudioByteSourceInternal];

		_currentAudioByteSourceInternal = (_currentAudioByteSourceInternal + 1) % AudioByteSourceInternalCount;

		return audioByteSource.Play(soundByte, gain, looping)!;
	}

	protected override void HandleCommand(AudioCommand command)
	{
		switch (command)
		{
			case BatchCommand(var commands):
				foreach (var singleCommand in commands)
					HandleCommand(singleCommand);
				break;
			case BindSourceBufferCommand(var source, var buffer):
				bindSourceBuffer(source, buffer);
				break;
			case BufferAndFreeDataCommand(var buffer, byte[] data, int dataLength, ALFormat format, int frequency):
				bufferAndFreeData(buffer, data, dataLength, format, frequency);
				break;
			case BufferDataCommand(var buffer, byte[] data, int dataLength, ALFormat format, int frequency):
				bufferData(buffer, data, dataLength, format, frequency);
				return;
			case CloseDeviceCommand(IntPtr device):
				closeDevice(device);
				break;
			case DecrementCounterCommand(var counter):
				decrementCounter(counter);
				break;
			case DeleteBufferCommand(var buffer):
				deleteBuffer(buffer);
				break;
			case DeleteSourceCommand(var source):
				deleteSource(source);
				break;
			case PauseSourceCommand(var source):
				pauseSource(source);
				break;
			case PlaySourceCommand(var source):
				playSource(source);
				break;
			case PrintErrorsCommand:
				printErrors();
				break;
			case QueueSourceBufferCommand(var source, var buffer):
				queueSourceBuffer(source, buffer);
				break;
			case ReopenDeviceCommand(IntPtr device, string deviceName, int[] attributes):
				reopenDevice(device, deviceName, attributes);
				break;
			case SetDistanceModelCommand(var distanceModel):
				setDistanceModel(distanceModel);
				break;
			case SetListenerGainCommand(var gain):
				setListenerGain(gain);
				break;
			case SetListenerPositionCommand(var position):
				setListenerPosition(position);
				break;
			case SetListenerVelocityCommand(var velocity):
				setListenerVelocity(velocity);
				break;
			case SetSourceGainCommand(var source, var gain):
				setSourceGain(source, gain);
				break;
			case SetSourceLoopingCommand(var source, var looping):
				setSourceLooping(source, looping);
				break;
			case SetSourcePitchCommand(var source, var pitch):
				setSourcePitch(source, pitch);
				break;
			case SetSourcePositionCommand(var source, var position):
				setSourcePosition(source, position);
				break;
			case SetSourceRelativeCommand(var source, var relative):
				setSourceRelative(source, relative);
				break;
			case SetSourceSecOffsetCommand(var source, var offset):
				setSourceSecOffset(source, offset);
				break;
			case SetSourceVelocityCommand(var source, var velocity):
				setSourceVelocity(source, velocity);
				break;
			case StopSourceCommand(var source):
				stopSource(source);
				break;
			case UnqueueAllSourceBuffersCommand(var source):
				unqueueAllSourceBuffers(source);
				break;
		}
	}

	public override void Update()
	{
		if (_device.IsConnected() == false)
		{
			_device.Reopen(null, _deviceAttributes);

			return;
		}

		foreach (var source in _audioSources)
			source.Update();

		foreach (var source in _audioByteSources)
			source.Update();
	}

	protected override void OnDispose()
	{
		_device.Dispose();
	}
}
