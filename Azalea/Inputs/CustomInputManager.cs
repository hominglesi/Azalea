using Azalea.Inputs.Handlers;
using System.Collections.Immutable;
using System.Linq;

namespace Azalea.Inputs;

public class CustomInputManager : InputManager
{
    protected override ImmutableArray<InputHandler> InputHandlers => _inputHandlers;

    private ImmutableArray<InputHandler> _inputHandlers = ImmutableArray.Create<InputHandler>();

    protected void AddHandler(InputHandler handler)
    {
        if (handler.Initialize(Host) == false) return;

        _inputHandlers = _inputHandlers.Append(handler).ToImmutableArray();
    }

    protected void RemoveHandler(InputHandler handler)
    {
        _inputHandlers.Where(h => h != handler).ToImmutableArray();
    }
}
