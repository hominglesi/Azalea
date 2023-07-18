namespace Azalea.Inputs.States;

public class InputState
{
    public readonly MouseState Mouse;

    public InputState(MouseState? mouse = null)
    {
        Mouse = mouse ?? new MouseState();
    }
}
