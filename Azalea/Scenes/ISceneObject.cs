using Azalea.Inputs.Events;

namespace Azalea.Scenes;

public interface ISceneObject
{
	public bool TriggerEvent(InputEvent e);
	public bool Hovered { get; internal set; }
	public bool HasFocus { get; internal set; }
	public bool AcceptsFocus { get; }
	public bool AcceptsFiles { get; }

}
