namespace Azalea.Inputs.Events;

public class KeyDownEvent(Keys key, bool isRepeat = false) : KeyEvent(key)
{
	public readonly bool IsRepeat = isRepeat;

	public void Deconstruct(out Keys key, out bool isRepeat)
	{
		key = Key;
		isRepeat = IsRepeat;
	}
}
