namespace Azalea.Inputs.Events;
public class ScrollEvent : InputEvent
{
	public float ScrollDelta;

	public ScrollEvent(float scrollDelta)
	{
		ScrollDelta = scrollDelta;
	}
}
