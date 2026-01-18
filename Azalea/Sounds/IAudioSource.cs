using System;

namespace Azalea.Sounds;
public interface IAudioSource
{
	public AudioSourceState State { get; }
	public Action<AudioSourceState>? StateUpdated { get; set; }

	public float Volume { get; set; }

	public void Pause();
	public void Unpause();
	public void Stop();
}

public enum AudioSourceState
{
	Stopped,
	Playing,
	Paused
}
