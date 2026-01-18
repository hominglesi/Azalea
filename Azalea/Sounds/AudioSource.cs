using Azalea.Utils;

namespace Azalea.Sounds;
internal abstract class AudioSource : Disposable
{
	private float _gain = 1;
	public float Gain
	{
		get => _gain;
		set
		{
			if (_gain == value) return;
			SetGainImplementation(value);
			_gain = value;
		}
	}

	protected abstract void SetGainImplementation(float gain);

	private bool _looping = false;
	public bool Looping
	{
		get => _looping;
		set
		{
			if (_looping == value) return;
			SetLoopingImplementation(value);
			_looping = value;
		}
	}
	protected abstract void SetLoopingImplementation(bool looping);

	private AudioInstanceLegacyAudio? _currentInstance;

	protected abstract void BindBufferImplementation(SoundByte sound);
	protected abstract void PlayImplementation();
	public AudioInstanceLegacyAudio Play(SoundByte sound, float gain = 1, bool looping = false)
	{
		Stop();

		BindBufferImplementation(sound);
		Gain = gain;
		Looping = looping;

		PlayImplementation();

		_currentInstance = new AudioInstanceLegacyAudio(this, sound);
		_currentInstance.Playing = true;
		return _currentInstance;
	}

	protected abstract void StopImplementation();
	public void Stop()
	{
		if (_currentInstance is null)
			return;

		_currentInstance.Playing = false;

		StopImplementation();

		_currentInstance = null;
	}
}
