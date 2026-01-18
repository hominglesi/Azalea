namespace Azalea.Sounds;
public interface IAudioInstance
{
	public void Stop();

	public float TotalDuration { get; }
	public float CurrentTimestamp { get; }
}
