using Azalea.Graphics.Rendering;
using Azalea.Graphics.Veldrid;
using System;

namespace Azalea.Platform.Veldrid;

public class VeldridGameHost : GameHost
{
    public override IWindow Window => _window;
    private readonly VeldridWindow _window;

    public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
    private VeldridRenderer? _renderer;

    private VeldridInputManager? _inputManager;

    public VeldridGameHost()
    {
        _window = new VeldridWindow();
        _window.OnInitialized += CallInitialized;
        _window.OnUpdate += CallOnUpdate;
        _window.OnRender += CallOnRender;
    }

    public override void CallInitialized()
    {
        _renderer = new VeldridRenderer(_window.GraphicsDevice, _window);

        _inputManager = new VeldridInputManager(_window.Window);

        base.CallInitialized();
    }

    public override void CallOnUpdate()
    {
        base.CallOnUpdate();

        _inputManager?.Update();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        _window.Run();
    }
}
