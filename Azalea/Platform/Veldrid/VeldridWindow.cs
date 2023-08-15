using Azalea.Extentions.ImageExtentions;
using Azalea.Graphics.Veldrid;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Azalea.Platform.Veldrid;

public class VeldridWindow : IWindow
{
    public GraphicsDevice GraphicsDevice { get; private set; }
    public Sdl2Window Window;

    public Action? OnInitialized;
    public Action? OnUpdate;
    public Action? OnRender;

    public VeldridWindow(Vector2Int preferredClientSize, WindowState preferredWindowState)
    {
        var windowCreateInfo = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = preferredClientSize.X,
            WindowHeight = preferredClientSize.Y,
            WindowTitle = IWindow.DefaultTitle,
            WindowInitialState = preferredWindowState.ToVeldridWindowState()
        };
        Window = VeldridStartup.CreateWindow(windowCreateInfo);

        GraphicsDeviceOptions options = new()
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true
        };
        GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options);

        Window.Resizable = true;

        Window.Resized += onResized;
    }

    private void onResized()
    {
        GraphicsDevice.ResizeMainWindow((uint)ClientSize.X, (uint)ClientSize.Y);
    }

    public void Run()
    {
        OnInitialized?.Invoke();
        while (Window.Exists)
        {
            Window.PumpEvents();
            OnUpdate?.Invoke();
            OnRender?.Invoke();
        }
    }

    public Vector2Int ClientSize
    {
        get => new(Window.Width, Window.Height);
        set { Window.Width = value.X; Window.Height = value.Y; }
    }
    public string Title { get => Window.Title; set => Window.Title = value; }

    public WindowState State
    {
        get => Window.WindowState.ToAzaleaWindowState();
        set => Window.WindowState = value.ToVeldridWindowState();
    }

    public unsafe void SetIconFromStream(Stream imageStream)
    {
        using var image = Image.Load<Rgba32>(imageStream);
        var pixels = image.CreateReadOnlyPixelSpan();

        var pitch = image.Width * sizeof(Rgba32);

        fixed (void* pixelSpan = pixels.Span)
        {
            var surface = VeldridExtentions.SDL_CreateRGBSurfaceFrom(pixelSpan, image.Width, image.Height, 32, pitch, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
            VeldridExtentions.SDL_SetWindowIcon(Window.SdlWindowHandle, surface);

            VeldridExtentions.SDL_FreeSurface(surface);
        }
    }
}
