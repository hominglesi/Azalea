using Azalea.Scenes;

namespace Azalea.Inputs.Events;

public class FocusLostEvent(ISceneObject? nextFocused) : InputEvent
{
	public readonly ISceneObject? NextFocused = nextFocused;
}
