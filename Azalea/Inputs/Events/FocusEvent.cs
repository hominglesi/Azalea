using Azalea.Scenes;

namespace Azalea.Inputs.Events;

public class FocusEvent(ISceneObject? previuosFocused) : InputEvent
{
	public readonly ISceneObject? PreviousFocused = previuosFocused;
}
