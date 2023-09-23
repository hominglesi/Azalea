using Azalea.Graphics;

namespace Azalea.Inputs.Events;
public class FocusLostEvent : InputEvent
{
	public readonly GameObject? NextFocused;

	public FocusLostEvent(GameObject? nextFocused)
	{
		NextFocused = nextFocused;
	}
}
