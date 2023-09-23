using System.Numerics;

namespace Azalea.Inputs.Events;

public class MouseDownEvent : MouseButtonEvent
{
	public MouseDownEvent(MouseButton button, Vector2 position)
		: base(button, position)
	{

	}
}
