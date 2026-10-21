using System.Numerics;

namespace Azalea.Inputs.Events;

public class MouseDownEvent(MouseButton button, Vector2 position)
	: MouseButtonEvent(button, position)
{ }
