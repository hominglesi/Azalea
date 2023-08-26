using Azalea.Inputs.States;

namespace Azalea.Inputs.Events;

public class KeyUpEvent : KeyEvent
{
	public KeyUpEvent(InputState state, Keys key)
		: base(state, key) { }
}
