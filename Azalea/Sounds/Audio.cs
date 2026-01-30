using Azalea.Platform;
using Azalea.Sounds.OpenAL;
using System;

namespace Azalea.Sounds;
public static class Audio
{
	private static IAudioManager? _instance;
	public static IAudioManager Instance => _instance ??= GameHost.Main.AudioManager;

	public static float MasterVolume
	{
		get => Instance.MasterVolume;
		set => Instance.MasterVolume = value;
	}

	public static IAudioInstance Play(Sound sound, float gain = 1, bool looping = false)
		=> Instance.Play(sound, gain, looping);

	public static IAudioInstance PlayByte(SoundByte soundByte, float gain = 1, bool looping = false)
		=> Instance.PlayByte(soundByte, gain, looping);

	internal static IAudioInstance PlayByteInternal(SoundByte soundByte, float gain = 1, bool looping = false)
		=> Instance.PlayByteInternal(soundByte, gain, looping);

	internal static SoundByte CreateSound(ReadOnlySpan<byte> data, ALFormat format, int frequency)
		=> Instance.CreateSoundByte(data, format, frequency);
}
