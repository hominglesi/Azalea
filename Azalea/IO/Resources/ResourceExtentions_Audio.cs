using Azalea.Sounds;
using System;

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

		WavSound wav;
		try { wav = new WavSound(stream); }
		catch (ArgumentException) { throw new ArgumentException("Sound Bytes only support .wav files"); }

		var sound = Audio.CreateSound(wav.Data, wav.Format, wav.Frequency);

		_soundByteCache.AddValue(store, path, sound);

		return sound;
	}
}
