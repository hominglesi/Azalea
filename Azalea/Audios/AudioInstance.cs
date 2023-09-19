using System;
using System.Diagnostics;

namespace Azalea.Audios;
public class AudioInstance
{
	public event Action? OnInstanceFinished;
	public bool IsPlaying => _isPlaying;
	private bool _isPlaying;

	private AudioSource? _source;

	public AudioInstance(AudioSource source)
	{
		_source = source;
		_isPlaying = true;
	}

	public void Stop()
	{
		if (_isPlaying)
		{
			_isPlaying = false;
			Debug.Assert(_source is not null);
			_source.Stop();
			_source = null;
			OnInstanceFinished?.Invoke();
		}
	}
}
