using Silk.NET.Maths;
using Silk.NET.Windowing;
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
        Window.WindowBorder = WindowBorder.Fixed;
    }

    public Vector2Int ClientSize
    {
        get => Window.Size;
        set => Window.Size = value;
    }

    public string Title
    {
        get => Window.Title;
        set => Window.Title = value;
    }
}
