using Azalea.Platform;

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

	public static AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
		=> Instance.Play(sound, gain, looping);

	public static AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false)
		=> Instance.PlayVital(sound, gain, looping);

	internal static AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false)
		=> Instance.PlayInternal(sound, gain, looping);

	internal static Sound CreateSound(ISoundData data)
		=> Instance.CreateSound(data);
}
