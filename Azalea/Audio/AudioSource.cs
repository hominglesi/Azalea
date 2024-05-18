using Azalea.Audio.OpenAL;
using Azalea.Utils;

namespace Azalea.Audio;
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

	private AudioInstance? _currentInstance;

	public AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
	{
		Stop();

		BindBufferImplementation(sound.Buffer);
		Gain = gain;
		Looping = looping;

		PlayImplementation();

		_currentInstance = new AudioInstance(this, sound);
		_currentInstance.Playing = true;
		return _currentInstance;
	}

	public void Stop()
	{
		if (_currentInstance is not null)
			_currentInstance.Playing = false;

		StopImplementation();

		_currentInstance = null;
	}

	protected abstract void BindBufferImplementation(ALBuffer buffer);
	protected abstract void PlayImplementation();
	protected abstract void StopImplementation();
}
