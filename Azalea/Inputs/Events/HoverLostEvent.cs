using Azalea.Inputs.States;

namespace Azalea.Inputs.Events;

public class HoverLostEvent : MouseEvent
{
	public HoverLostEvent(InputState state)
		: base(state) { }

}
