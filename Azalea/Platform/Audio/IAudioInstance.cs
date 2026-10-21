using static Azalea.Platform.Audio.PlatformAudio;

namespace Azalea.Platform.Audio;

public partial interface IAudioInstance
{
	internal PlatformAudio Owner { get; }
	public bool Looping { get; }
	public double Duration { get; }

	public AudioInstanceState State { get; }
	public float Gain { get; }
	public float Timestamp { get; }

	public void SetGain(float gain) => Owner.Enqueue(SetInstanceGainCommand.Borrow(this, gain));
	public void SetTimestamp(float timestamp) => Owner.Enqueue(SetInstanceTimestampCommand.Borrow(this, timestamp));
	public void Pause() => Owner.Enqueue(PauseInstanceCommand.Borrow(this));
	public void Unpause() => Owner.Enqueue(UnpauseInstanceCommand.Borrow(this));
	public void Stop() => Owner.Enqueue(StopInstanceCommand.Borrow(this));
}

public enum AudioInstanceState
{
	Playing,
	Paused,
	Stopped
}
