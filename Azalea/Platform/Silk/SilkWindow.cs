using Silk.NET.Core;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Runtime.InteropServices;
using IWindowSilk = Silk.NET.Windowing.IWindow;
using WindowSilk = Silk.NET.Windowing.Window;

namespace Azalea.Platform.Silk;

internal class SilkWindow : IWindow
{
    public IWindowSilk Window;

    public SilkWindow(Vector2Int preferredClientSize)
    {
        var windowOptions = WindowOptions.Default with
        {
            Size = new Vector2D<int>(preferredClientSize.X, preferredClientSize.Y),
            Title = IWindow.DefaultTitle,
            WindowBorder = WindowBorder.Fixed
        };

        Window = WindowSilk.Create(windowOptions);
    }

    public unsafe void SetIconFromStream(Stream imageStream)
    {
        using var image = Image.Load<Rgba32>(imageStream);

        var memoryGroup = image.GetPixelMemoryGroup();
        Memory<byte> array = new byte[memoryGroup.TotalLength * sizeof(Rgba32)];
        var block = MemoryMarshal.Cast<byte, Rgba32>(array.Span);
        foreach (var memory in memoryGroup)
        {
            memory.Span.CopyTo(block);
            block = block[memory.Length..];
        }

        var icon = new RawImage(image.Width, image.Height, array);

        Window.SetWindowIcon(ref icon);
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

    public WindowState State
    {
        get => Window.WindowState.ToAzaleaWindowState();
        set => Window.WindowState = value.ToSilkWindowState();
    }

    public bool Resizable
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public bool CursorVisible
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
}
