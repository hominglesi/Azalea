namespace Azalea.Sounds;
internal class SoundByteInstance : IAudioInstance
{
	private readonly IAudioSource _source;
	public float TotalDuration { get; init; }

	public SoundByteInstance(IAudioSource source, float duration)
	{
		_source = source;
		TotalDuration = duration;
	}

	public float CurrentTimestamp => _source.CurrentTimestamp;

	public void Stop()
	{
		if (_source.CurrentInstance == this)
			_source.Stop();
	}
}
