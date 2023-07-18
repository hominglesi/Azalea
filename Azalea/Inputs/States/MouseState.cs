using System.Numerics;

namespace Azalea.Inputs.States;

public class MouseState
{
    public readonly ButtonStates<MouseButton> Buttons = new();

    public Vector2 Position;

    public bool IsPressed(MouseButton button) => Buttons.IsPressed(button);
}
