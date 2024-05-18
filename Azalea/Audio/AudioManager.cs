using Azalea.Audio.OpenAL;
using System;

namespace Azalea.Audio;
public static class AudioManager
{
	private const int SourceCount = 32;

	private static ALC_Device _device;
	private static ALC_Context _context;

	private static ALSource[] _sources = new ALSource[SourceCount];

	private static float _masterVolume = 1.0f;
	public static float MasterVolume
	{
		get => _masterVolume;
		set
		{
			if (_masterVolume == value) return;

			ALC.SetListenerGain(value);

			_masterVolume = value;
		}
	}

	internal static void Initialize()
	{
		_device = ALC.OpenDevice();
		_context = ALC.CreateContext(_device);
		ALC.MakeContextCurrent(_context);

		for (int i = 0; i < SourceCount; i++)
		{
			_sources[i] = new ALSource();
		}
	}

	private static AudioInstance playOnChannel(int channel, Sound sound, float gain, bool looping)
	{
		if (sound is null)
		{
			Console.WriteLine("The played sound was null");
			return null;
		}

		return _sources[channel].Play(sound, gain, looping);
	}

	internal static AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false)
	{
		return playOnChannel(31, sound, gain, looping);
	}

	private const int _vitalChannels = 4;
	private static int _currentVitalChannel = 0;
	public static AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false)
	{
		var played = playOnChannel(_currentVitalChannel, sound, gain, looping);

		_currentVitalChannel += 1;
		if (_currentVitalChannel >= _vitalChannels)
			_currentVitalChannel = 0;

		return played;
	}

	private const int _audioChannels = SourceCount - _vitalChannels - 1;
	private static int _currentAudioChannel = _vitalChannels;
	public static AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
	{
		var played = playOnChannel(_currentAudioChannel, sound, gain, looping);

		_currentAudioChannel += 1;
		if (_currentAudioChannel >= _audioChannels)
			_currentAudioChannel = _vitalChannels;

		return played;
	}

	internal static void Dispose()
	{
		ALC.CloseDevice(_device);
	}
}
