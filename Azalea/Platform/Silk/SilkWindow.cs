using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
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

	private GL? _gl;
	private SilkInputManager? _input;

	public SilkWindow(Vector2Int preferredClientSize)
	{
		var windowOptions = WindowOptions.Default with
		{
			Size = new Vector2D<int>(preferredClientSize.X, preferredClientSize.Y),
			Title = IWindow.DefaultTitle,
			WindowBorder = WindowBorder.Fixed
		};

		Window = WindowSilk.Create(windowOptions);
		Window.Resize += onResize;
	}

	private void onResize(Vector2D<int> obj)
	{
		if (_gl is null) return;
		_gl.Viewport(0, 0, (uint)ClientSize.X, (uint)ClientSize.Y);
	}

	public void InitializeAfterStartup(GL gl, SilkInputManager input)
	{
		_gl = gl;
		_input = input;
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
		get
		{
			return Window.WindowBorder switch
			{
				WindowBorder.Resizable => true,
				_ => false
			};
		}
		set
		{
			if (value == false && Window.WindowBorder != WindowBorder.Fixed)
				Window.WindowBorder = WindowBorder.Fixed;
			else if (value == true && Window.WindowBorder != WindowBorder.Resizable)
				Window.WindowBorder = WindowBorder.Resizable;
		}
	}

	public bool CursorVisible
	{
		get
		{
			if (_input is null) return false;
			return _input.PrimaryMouse.Cursor.CursorMode switch
			{
				CursorMode.Hidden => false,
				_ => true
			};
		}
		set
		{
			if (_input is null) return;
			if (value == true && _input.PrimaryMouse.Cursor.CursorMode != CursorMode.Normal)
				_input.PrimaryMouse.Cursor.CursorMode = CursorMode.Normal;
			else if (value == false && _input.PrimaryMouse.Cursor.CursorMode != CursorMode.Hidden)
				_input.PrimaryMouse.Cursor.CursorMode = CursorMode.Hidden;
		}
	}

	public void Close()
	{
		Window.Close();
	}
}
