using System.Numerics;

namespace Azalea.Inputs.Events;

public class MouseButtonEvent : InputEvent
{
	public readonly MouseButton Button;

	public readonly Vector2 Position;

	protected MouseButtonEvent(MouseButton button, Vector2 position)
	{
		Button = button;
		Position = position;
	}
}
