using System.Numerics;

namespace Azalea.Inputs.Events;

public class MouseButtonEvent(MouseButton button, Vector2 position) : InputEvent
{
	public readonly MouseButton Button = button;

	public readonly Vector2 Position = position;

	public void Deconstruct(out MouseButton button, out Vector2 position)
	{
		button = Button;
		position = Position;
	}
}
