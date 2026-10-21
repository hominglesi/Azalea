using Azalea.Graphics;

namespace Azalea.Inputs.Events;

public class FocusEvent(GameObject? previuosFocused) : InputEvent
{
	public readonly GameObject? PreviousFocused = previuosFocused;
}
