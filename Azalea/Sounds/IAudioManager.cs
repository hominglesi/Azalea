namespace Azalea.Sounds;
public interface IAudioManager
{
	public float MasterVolume { get; set; }

	public AudioInstance Play(Sound sound, float gain = 1, bool looping = false);

	public AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false);

	internal AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false);

	internal Sound CreateSound(ISoundData data);
}
