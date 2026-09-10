using System;

namespace Azalea.Platform.Audio;
public interface IAudioInstance
{
	internal PlatformAudio Owner { get; }
	public double Duration { get; }

	public event Action Stopped;
}

public enum AudioInstanceState
{
	Playing,
	Paused,
	Stopped
}
