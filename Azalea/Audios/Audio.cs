using Azalea.IO.Assets;
using Silk.NET.OpenAL;
using System;

namespace Azalea.Audios;
public static unsafe class Audio
{
	internal static AL Al => _al ?? throw new InvalidOperationException("Audio was not initialized");
	internal static AL _al;

	private static ALContext _alc;
	private static Context* _context;
	private static Device* _device;
	internal static AudioSource[] Sources = new AudioSource[32];
	internal static int SourceIndex;

	public static AudioInstance Play(Sound sound)
	{
		Sources[SourceIndex].Stop();

		var instance = Sources[SourceIndex].Play(sound);

		SourceIndex++;
		if (SourceIndex >= 32) SourceIndex = 0;

		return instance;
	}

	internal static void Initialize()
	{
		_alc = ALContext.GetApi(true);
		_al = AL.GetApi(true);
		_device = _alc.OpenDevice("");
		if (_device == null)
		{
			Console.WriteLine("Could not create device");
			return;
		}

		_context = _alc.CreateContext(_device, null);
		_alc.MakeContextCurrent(_context);

		Al.GetError();

		for (int i = 0; i < 32; i++)
		{
			Sources[i] = new AudioSource(Al.GenSource(), Al);
		}
	}


	public static bool Disposed;
	public static void Dispose()
	{
		if (Disposed == false)
		{
			foreach (var sound in Assets.LoadedSounds)
			{
				sound.Dispose();
			}
			foreach (var source in Sources)
			{
				source.Dispose();
			}

			_alc.DestroyContext(_context);
			_alc.CloseDevice(_device);
			Al.Dispose();
			_alc.Dispose();

			Disposed = true;
		}
	}
}
