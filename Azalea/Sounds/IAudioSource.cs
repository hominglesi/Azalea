using System;

namespace Azalea.Sounds;
public interface IAudioSource
{
	public AudioSourceState State { get; }
	public Action<AudioSourceState>? StateUpdated { get; set; }
	public IAudioInstance? CurrentInstance { get; }
	public float CurrentTimestamp { get; }

	public float Volume { get; set; }

	public void Pause();
	public void Unpause();
	public void Stop();
	public void Seek(float timestamp);
}

public enum AudioSourceState
{
	Stopped,
	Playing,
	Paused
}
