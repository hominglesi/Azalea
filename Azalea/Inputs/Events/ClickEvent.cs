using Azalea.Inputs.States;
using System.Numerics;

namespace Azalea.Inputs.Events;

public class ClickEvent : MouseButtonEvent
{
	public ClickEvent(InputState state, MouseButton button, Vector2? screenSpaceMouseDownPosition = null)
		: base(state, button, screenSpaceMouseDownPosition) { }
}
