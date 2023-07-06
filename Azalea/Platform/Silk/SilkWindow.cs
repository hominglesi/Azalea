using Silk.NET.Maths;
using Silk.NET.Windowing;
using System.Numerics;
using IWindowSilk = Silk.NET.Windowing.IWindow;
using WindowSilk = Silk.NET.Windowing.Window;

namespace Azalea.Platform.Silk;

internal class SilkWindow : IWindow
{
    public IWindowSilk Window;

    public SilkWindow()
    {
        var windowOptions = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Game"
        };

        Window = WindowSilk.Create(windowOptions);
    }

    public Vector2 ClientSize => new(Window.Size.X, Window.Size.Y);

    public string Title
    {
        get => Window.Title;
        set => Window.Title = value;
    }
}
