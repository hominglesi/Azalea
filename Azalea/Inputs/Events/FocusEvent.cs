using Azalea.Graphics;

namespace Azalea.Inputs.Events;
public class FocusEvent : InputEvent
{
	public readonly GameObject? PreviousFocused;

	public FocusEvent(GameObject? previuosFocused)
	{
		PreviousFocused = previuosFocused;
	}
}
