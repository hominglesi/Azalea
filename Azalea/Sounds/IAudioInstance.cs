namespace Azalea.Sounds;
public interface IAudioInstance
{
	public IAudioSource? Source { get; }

	public float TotalDuration { get; }
	public float CurrentTimestamp { get; }
	public float Volume { get; set; }

	public void Pause();
	public void Unpause();
	public void Stop();
	public void Seek(float timestamp);
}
