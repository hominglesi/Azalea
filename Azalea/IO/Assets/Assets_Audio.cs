using Azalea.Audio;
using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Assets;
public static partial class Assets
{
	internal static Dictionary<string, Sound> LoadedSounds = new();

	public static Sound GetSound(string path)
	{
		if (LoadedSounds.ContainsKey(path))
			return LoadedSounds[path];

		var wav = new WavSound(File.OpenRead(path));
		var sound = new Sound(wav);
		LoadedSounds.Add(path, sound);
		return sound;
	}

	private static void disposeAudio()
	{
		foreach (var sound in LoadedSounds.Values)
		{
			sound.Dispose();
		}
	}
}
