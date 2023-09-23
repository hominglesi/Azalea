using System.Numerics;

namespace Azalea.Inputs.Events;

public class MouseUpEvent : MouseButtonEvent
{
	public MouseUpEvent(MouseButton button, Vector2 position)
		: base(button, position)
	{

	}
}
