using Azalea.Inputs.States;

namespace Azalea.Inputs.Events;

public abstract class KeyEvent : UIEvent
{
	public readonly Keys Key;

	protected KeyEvent(InputState state, Keys key)
		: base(state)
	{
		Key = key;
	}
}
