using System;

namespace Azalea.Sounds.OpenAL;
internal class ALAudioManager : AudioManager
{
	// These should add up to 32
	protected const int AudioSourceCount = 4;
	protected const int AudioByteSourceCount = 24;
	protected const int AudioByteSourceInternalCount = 4;

	private readonly ALC_Device _device;
	private readonly ALC_Context _context;

	private readonly ALAudioSource[] _audioSources;
	private readonly ALAudioByteSource[] _audioByteSources;
	private readonly ALAudioByteSource[] _audioByteSourcesInternal;

	public override IAudioSource[] AudioChannels => _audioSources;
	public override IAudioSource[] AudioByteChannels => _audioByteSources;
	public override IAudioSource[] AudioByteInternalChannels => _audioByteSourcesInternal;

	public ALAudioManager()
	{
		_device = ALC.OpenDevice();
		_context = ALC.CreateContext(_device);
		ALC.MakeContextCurrent(_context);

		_audioSources = new ALAudioSource[AudioSourceCount];
		for (int i = 0; i < AudioSourceCount; i++)
			_audioSources[i] = new ALAudioSource();

		_audioByteSources = new ALAudioByteSource[AudioByteSourceCount];
		for (int i = 0; i < AudioByteSourceCount; i++)
			_audioByteSources[i] = new ALAudioByteSource();

		_audioByteSourcesInternal = new ALAudioByteSource[AudioByteSourceInternalCount];
		for (int i = 0; i < AudioByteSourceInternalCount; i++)
			_audioByteSourcesInternal[i] = new ALAudioByteSource();
	}

	public override SoundByte CreateSoundByte(ReadOnlySpan<byte> data, ALFormat format, int frequency)
		=> new ALSound(data, format, frequency);
	protected override void SetMasterVolumeImplementation(float volume)
		=> ALC.SetListenerGain(volume);

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

	public override void Update()
	{
		foreach (var source in _audioSources)
			source.Update();

		foreach (var source in _audioByteSources)
			source.Update();
	}

	protected override void OnDispose()
	{
		ALC.CloseDevice(_device);
	}
}
