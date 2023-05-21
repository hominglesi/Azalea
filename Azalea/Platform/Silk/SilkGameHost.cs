using Azalea.Graphics.OpenGL;
using Azalea.Graphics.Rendering;
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

    public override event Action? Initialized;

    public SilkGameHost()
    {
        var windowOptions = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Game"
        };

        _window = Window.Create(windowOptions);
        _window.Load += onLoad;
        _window.Render += onRender;
    }

    private void onLoad()
    {
        _gl = _window.CreateOpenGL();
        _renderer = new GLRenderer(_gl);

        Initialized?.Invoke();
    }

    private void onRender(double deltaTime)
    {
        Renderer.Clear();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        _window.Run();
    }


}
