namespace Azalea.Platform.Audio;

public static class Audio
{
	public static float MasterVolume
	{
		get => GameHost.Instance.Audio.MasterVolume;
		set => GameHost.Instance.Audio.MasterVolume = value;
	}

	public static IAudioInstance Play(Sound sound, float gain = 1, bool looping = false)
		=> GameHost.Instance.Audio.Play(sound, gain, looping);

	public static IAudioInstance PlayByte(SoundByte soundByte, float gain = 1, bool looping = false)
		=> GameHost.Instance.Audio.PlayByte(soundByte, gain, looping);
}
