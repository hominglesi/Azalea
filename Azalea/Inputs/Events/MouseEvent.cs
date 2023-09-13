using Azalea.Inputs.States;

namespace Azalea.Inputs.Events;

public class MouseEvent : UIEvent
{
	public bool IsPressed(MouseButton button) => CurrentState.Mouse.IsPressed(button);

	public MouseEvent(InputState state)
		: base(state) { }
}
