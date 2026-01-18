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

	protected abstract void BindBufferImplementation(SoundByte sound);
	protected abstract void PlayImplementation();

	protected abstract void StopImplementation();
}
