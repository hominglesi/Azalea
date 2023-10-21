namespace Azalea.Amends;
public interface IAmend
{
	public bool IsFinished { get; }
	public bool HasStarted { get; }
	public void Start();
	public void Update(float deltaTime);
}
