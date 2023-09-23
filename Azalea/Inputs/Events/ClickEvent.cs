using System.Numerics;

namespace Azalea.Inputs.Events;
public class ClickEvent : MouseButtonEvent
{
	public ClickEvent(MouseButton button, Vector2 position)
		: base(button, position)
	{

	}
}
