namespace Azalea.Sounds;
public interface IAudioManager
{
	public float MasterVolume { get; set; }

	public AudioInstanceLegacyAudio PlayLegacyAudio(SoundByte sound, float gain = 1, bool looping = false);
	public AudioInstanceLegacyAudio PlayVitalLegacyAudio(SoundByte sound, float gain = 1, bool looping = false);
	internal AudioInstanceLegacyAudio PlayInternalLegacyAudio(SoundByte sound, float gain = 1, bool looping = false);

	public IAudioSource[] AudioChannels { get; }
	public int AudioByteChannels { get; }
	public int AudioByteInternalChannels { get; }

	public IAudioInstance PlayAudio(Sound sound, float gain = 1, bool looping = false);
	public IAudioInstance PlayAudioByte(SoundByte soundByte, float gain = 1, bool looping = false);
	public IAudioInstance PlayAudioByteInternal(SoundByte soundByte, float gain = 1, bool looping = false);
	internal SoundByte CreateSoundByte(ISoundData data);

	internal void Update();
}
