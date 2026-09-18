using Azalea.Utils;
using System;

namespace Azalea.Platform.Audio;
public partial interface IAudioInstance
{
	internal PlatformAudio Owner { get; }
	public bool Looping { get; }
	public double Duration { get; }
	
	public AudioInstanceState State { get; }
	public float Gain { get; }
	public float Timestamp { get; }

	public void Pause() => Owner.PauseInstance(this);
	public void SetGain(float gain) => Owner.SetInstanceGain(this, gain);
	public void SetTimestamp(float timestamp) => Owner.SetInstanceTimestamp(this, timestamp);
	public void Stop() => Owner.StopInstance(this);
	public void Unpause() => Owner.UnpauseInstance(this);
}

public enum AudioInstanceState
{
	Playing,
	Paused,
	Stopped
}
