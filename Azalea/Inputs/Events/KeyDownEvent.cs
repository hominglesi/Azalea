using Azalea.Inputs.States;

namespace Azalea.Inputs.Events;

public class KeyDownEvent : KeyEvent
{
	public readonly bool IsRepeat;

	public KeyDownEvent(InputState state, Keys key, bool isRepeat = false)
		: base(state, key)
	{
		IsRepeat = isRepeat;
	}
}
