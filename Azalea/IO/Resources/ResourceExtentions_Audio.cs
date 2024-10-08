using Azalea.Sounds;
using System;
using System.IO;

namespace Azalea.IO.Resources;
public static partial class ResourceStoreExtentions
{
	private static ResourceCache<Sound> _audioCache = new();

	public static Sound GetSound(this IResourceStore store, string path)
	{
		if (_audioCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("Sound could not be found.");

		var sound = getSound(stream);
		_audioCache.AddValue(store, path, sound);

		return sound;
	}

	private static Sound getSound(Stream stream)
	{
		var wav = new WavSound(stream);
		var sound = Audio.CreateSound(wav);
		return sound;
	}
}
