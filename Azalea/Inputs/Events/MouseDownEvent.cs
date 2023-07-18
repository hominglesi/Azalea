using Azalea.Inputs.States;
using System.Numerics;

namespace Azalea.Inputs.Events;

public class MouseDownEvent : MouseButtonEvent
{
    public MouseDownEvent(InputState state, MouseButton button, Vector2? screenSpaceMouseDownPosition = null)
        : base(state, button, screenSpaceMouseDownPosition) { }
}
