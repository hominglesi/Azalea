using Azalea.Inputs.States;

namespace Azalea.Inputs.Events;

public class HoverEvent : MouseEvent
{
	public HoverEvent(InputState state)
		: base(state) { }
}
