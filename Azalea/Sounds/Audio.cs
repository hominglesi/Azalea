using Azalea.Platform;

namespace Azalea.Sounds;
public static class Audio
{
	public static IAudioManager Instance => GameHost.Main.AudioManager;

	public static float MasterVolume
	{
		get => Instance.MasterVolume;
		set => Instance.MasterVolume = value;
	}

	public static AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
		=> Instance.Play(sound, gain, looping);

	public static AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false)
		=> Instance.PlayVital(sound, gain, looping);
}
