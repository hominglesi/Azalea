using Azalea.Inputs.States;

namespace Azalea.Inputs.Events;

public class KeyDownEvent : KeyEvent
{
	public KeyDownEvent(InputState state, Keys key)
		: base(state, key) { }
}
