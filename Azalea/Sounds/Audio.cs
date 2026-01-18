using Azalea.Platform;
using Azalea.Sounds.OpenAL;

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

	public static IAudioInstance PlayAudio(Sound sound, float gain = 1, bool looping = false)
		=> Instance.PlayAudio(sound, gain, looping);

	public static IAudioInstance PlayAudioByte(SoundByte soundByte, float gain = 1, bool looping = false)
		=> Instance.PlayAudioByte(soundByte, gain, looping);

	internal static IAudioInstance PlayAudioByteInternal(SoundByte soundByte, float gain = 1, bool looping = false)
		=> Instance.PlayAudioByteInternal(soundByte, gain, looping);

	internal static SoundByte CreateSound(byte[] data, ALFormat format, int frequency)
		=> Instance.CreateSoundByte(data, format, frequency);
}
