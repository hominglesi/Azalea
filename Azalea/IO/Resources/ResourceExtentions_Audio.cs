using Azalea.Sounds;
using System;
using System.IO;

namespace Azalea.IO.Resources;
public static partial class ResourceStoreExtentions
{
	private static ResourceCache<Sound> _soundCache = new();

	public static Sound GetSound(this IResourceStore store, string path)
	{
		if (_soundCache.TryGetValue(store, path, out var cached))
			return cached;

		var sound = new Sound(store, path);
		_soundCache.AddValue(store, path, sound);
		return sound;
	}

	private static ResourceCache<SoundByte> _soundByteCache = new();

	public static SoundByte GetSoundByte(this IResourceStore store, string path)
	{
		if (_soundByteCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("Sound could not be found.");

		var sound = getSound(stream);
		_soundByteCache.AddValue(store, path, sound);

		return sound;
	}

	private static ResourceCache<SoundByte> _audioCacheLegacyAudio = new();

	public static SoundByte GetSoundLegacyAudio(this IResourceStore store, string path)
	{
		if (_audioCacheLegacyAudio.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("Sound could not be found.");

		var sound = getSound(stream);
		_audioCacheLegacyAudio.AddValue(store, path, sound);

		return sound;
	}

	private static SoundByte getSound(Stream stream)
	{
		var wav = new WavSound(stream);
		var sound = Audio.CreateSound(wav);
		return sound;
	}
}
