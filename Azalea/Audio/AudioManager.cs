using Azalea.Audio.OpenAL;
using System;

namespace Azalea.Audio;
public static class AudioManager
{
	private const int SourceCount = 32;

	private static ALC_Device _device;
	private static ALC_Context _context;

	private static ALSource[] _sources = new ALSource[SourceCount];

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

	public static AudioInstance Play(Sound sound, float gain = 1)
	{
		if (sound is null)
		{
			Console.WriteLine("The played sound was null");
			return null;
		}

		return _sources[0].Play(sound, gain);
	}

	internal static void Dispose()
	{
		ALC.CloseDevice(_device);
	}
}
