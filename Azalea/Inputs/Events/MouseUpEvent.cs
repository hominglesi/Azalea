using Azalea.Inputs.States;
using System.Numerics;

namespace Azalea.Inputs.Events;

public class MouseUpEvent : MouseButtonEvent
{
	public MouseUpEvent(InputState state, MouseButton button, Vector2? screenSpaceMouseDownPosition = null)
		: base(state, button, screenSpaceMouseDownPosition) { }
}