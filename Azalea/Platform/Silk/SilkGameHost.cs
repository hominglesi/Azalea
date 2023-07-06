using Azalea.Graphics.OpenGL;
using Azalea.Graphics.Rendering;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;

namespace Azalea.Platform.Silk;

internal class SilkGameHost : GameHost
{
    public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
    private SilkWindow _window;

    public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
    private GLRenderer? _renderer;

    private GL Gl => _gl ?? throw new Exception("Cannot use GL before it is initialized");
    private GL? _gl;

    private SilkInputManager? _inputManager;

    public SilkGameHost()
    {
        _window = new SilkWindow();

        _window.Window.Load += CallInitialized;
        _window.Window.Render += (_) => CallOnRender();
        _window.Window.Update += (_) => CallOnUpdate();
    }

    public override void CallInitialized()
    {
        _gl = _window.Window.CreateOpenGL();
        _renderer = new GLRenderer(_gl, _window.Window);

        _inputManager = new SilkInputManager(_window.Window.CreateInput());

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
        _window.Window.Run();
    }


}
