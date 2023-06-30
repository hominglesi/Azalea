using Azalea.Graphics.OpenGL;
using Azalea.Graphics.Rendering;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Azalea.Platform.Silk;

internal class SilkGameHost : GameHost
{
    public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
    private GLRenderer? _renderer;

    private readonly IWindow _window;

    private GL Gl => _gl ?? throw new Exception("Cannot use GL before it is initialized");
    private GL? _gl;

    private SilkInputManager? _inputManager;

    public SilkGameHost()
    {
        var windowOptions = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Game"
        };

        _window = Window.Create(windowOptions);
        _window.Load += CallInitialized;
        _window.Render += (_) => CallOnRender();
    }

    public override void CallInitialized()
    {
        _gl = _window.CreateOpenGL();
        _renderer = new GLRenderer(_gl, _window);

        _inputManager = new SilkInputManager(_window.CreateInput());

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
