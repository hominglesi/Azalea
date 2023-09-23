namespace Azalea.Inputs.Events;

public class KeyDownEvent : KeyEvent
{
	public readonly bool IsRepeat;

	public KeyDownEvent(Keys key, bool isRepeat = false)
		: base(key)
	{
		IsRepeat = isRepeat;
	}
}
