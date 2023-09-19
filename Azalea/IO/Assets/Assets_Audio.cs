using Azalea.Audios;
using System;
using System.Collections.Generic;

namespace Azalea.IO.Assets;
public static partial class Assets
{
	internal static List<Sound> LoadedSounds = new List<Sound>();

	public static Sound GetSound(string path)
	{
		if (Audio.Al is null) throw new InvalidOperationException("Assets was not initialized properly");
		var newSound = new Sound(path, Audio.Al);
		LoadedSounds.Add(newSound);
		return newSound;
	}
}
