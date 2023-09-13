namespace Azalea.Inputs;

public abstract class MouseButtonEventManager : ButtonEventManager<MouseButton>
{
	public MouseButtonEventManager(MouseButton button)
		: base(button) { }
}
