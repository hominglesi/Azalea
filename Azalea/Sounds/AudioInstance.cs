namespace Azalea.Sounds;
internal class AudioInstance(IAudioSource source, float duration) : IAudioInstance
{
	private readonly IAudioSource _source = source;
	public IAudioSource? Source => _source.CurrentInstance == this ? _source : null;

	public float TotalDuration { get; } = duration;
	public float CurrentTimestamp => Source is not null ? Source.CurrentTimestamp : 0;

	public float Volume
	{
		get => Source is not null ? Source.Volume : 0;
		set { if (Source is not null) Source.Volume = value; }
	}

	public float Pitch
	{
		get => Source is not null ? Source.Pitch : 0;
		set { if (Source is not null) Source.Pitch = value; }
	}

	public bool Looping
	{
		get => Source is not null ? Source.Looping : false;
		set { if (Source is not null) Source.Looping = value; }
	}

	public void Pause() => Source?.Pause();
	public void Unpause() => Source?.Unpause();
	public void Stop() => Source?.Stop();
	public void Seek(float timestamp) => Source?.Seek(timestamp);
}
