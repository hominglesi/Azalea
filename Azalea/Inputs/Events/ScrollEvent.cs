namespace Azalea.Inputs.Events;
public class ScrollEvent(float delta) : InputEvent
{
	public float Delta = delta;

	public void Deconstruct(out float delta)
	{
		delta = Delta;
	}
}
