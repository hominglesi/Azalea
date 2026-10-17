using Azalea.Graphics;

namespace Azalea.Inputs.Events;

public class FocusLostEvent(GameObject? nextFocused) : InputEvent
{
	public readonly GameObject? NextFocused = nextFocused;
}
