using System.Diagnostics;

namespace Azalea.Sounds.OpenAL;
internal class ALAudioManager : AudioManager
{
	protected const int SourceCount = 32;

	private readonly ALC_Device _device;
	private readonly ALC_Context _context;
	private readonly ALSource[] _sources;

	public ALAudioManager()
	{
		_device = ALC.OpenDevice();
		_context = ALC.CreateContext(_device);
		ALC.MakeContextCurrent(_context);

		_sources = new ALSource[SourceCount];
		for (int i = 0; i < SourceCount; i++)
		{
			_sources[i] = new ALSource();
		}
	}

	public override Sound CreateSound(ISoundData data)
		=> new ALSound(data);
	protected override void SetMasterVolumeImplementation(float volume)
		=> ALC.SetListenerGain(volume);

	private AudioInstance playOnChannel(int channel, Sound sound, float gain, bool looping)
	{
		Debug.Assert(sound is not null);

		return _sources[channel].Play(sound, gain, looping);
	}

	public override AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false)
		=> playOnChannel(SourceCount - 1, sound, gain, looping);

	private const int _vitalChannels = 4;
	private int _currentVitalChannel = 0;
	public override AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false)
	{
		var played = playOnChannel(_currentVitalChannel, sound, gain, looping);

		_currentVitalChannel += 1;
		if (_currentVitalChannel >= _vitalChannels)
			_currentVitalChannel = 0;

		return played;
	}

	private const int _audioChannels = SourceCount - _vitalChannels - 1;
	private int _currentAudioChannel = _vitalChannels;
	public override AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
	{
		var played = playOnChannel(_currentAudioChannel, sound, gain, looping);

		_currentAudioChannel += 1;
		if (_currentAudioChannel >= _audioChannels)
			_currentAudioChannel = _vitalChannels;

		return played;
	}

	protected override void OnDispose()
	{
		ALC.CloseDevice(_device);
	}
}
