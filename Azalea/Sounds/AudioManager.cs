using Azalea.Utils;

namespace Azalea.Sounds;
internal abstract class AudioManager : Disposable, IAudioManager
{
	private float _masterVolume = 1.0f;
	protected abstract void SetMasterVolumeImplementation(float volume);
	public float MasterVolume
	{
		get => _masterVolume;
		set
		{
			if (_masterVolume == value) return;

			SetMasterVolumeImplementation(value);

			_masterVolume = value;
		}
	}

	public abstract Sound CreateSound(ISoundData data);
	public abstract AudioInstance Play(Sound sound, float gain = 1, bool looping = false);
	public abstract AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false);
	public abstract AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false);
}
