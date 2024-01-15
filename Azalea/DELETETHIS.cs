using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Platform.Windows;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;

namespace Azalea;
public static class DELETETHIS
{
	private static IntPtr _deviceContext;

	public static void Run()
	{
		var programHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		var cursor = WinAPI.LoadCursor(IntPtr.Zero, 32512);

		var wndClass = new WindowClass("Azalea Window", programHandle, procedure)
		{
			Style = ClassStyles.OwnDC,
			Cursor = cursor
		};
		var id = WinAPI.RegisterClass(ref wndClass);

		var window = WinAPI.CreateWindow(
			0,
			id,
			"Azalea Window",
			WindowStyles.OverlappedWindow,
			100,
			100,
			1280,
			720,
			IntPtr.Zero,
			IntPtr.Zero,
			programHandle,
			IntPtr.Zero);

		if (window == IntPtr.Zero)
		{
			Console.WriteLine(Marshal.GetLastWin32Error());
			return;
		}

		WinAPI.ShowWindow(window, ShowWindowCommand.Show);

		_deviceContext = WinAPI.GetDC(window);

		var pfDescriptor = new PixelFormatDescriptor();
		var format = WinAPI.ChoosePixelFormat(_deviceContext, ref pfDescriptor);

		WinAPI.SetPixelFormat(_deviceContext, format, ref pfDescriptor);

		var glContext = GL.CreateContext(_deviceContext);
		GL.MakeCurrent(_deviceContext, glContext);

		drawTriangle();

		while (WinAPI.GetMessage(out Message message, window) > 0)
		{
			WinAPI.TranslateMessage(ref message);
			WinAPI.DispatchMessage(ref message);
		}
	}

	private static void drawTriangle()
	{
		GL.Clear(GLBufferBit.Color);

		GL.Begin(GLBeginMode.Triangles);
		GL.Vertex2f(-0.5f, -0.5f);
		GL.Vertex2f(0.0f, 0.5f);
		GL.Vertex2f(0.5f, 0.0f);
		GL.End();

		WinAPI.SwapBuffers(_deviceContext);
	}

	private static IntPtr procedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{

		switch ((WindowMessage)msg)
		{
			case WindowMessage.Size:

				var size = BitwiseUtils.SplitValue(lParam);

				GL.Viewport(0, 0, size.X, size.Y);
				drawTriangle();
				return IntPtr.Zero;
		}

		return WinAPI.DefWindowProc(hWnd, msg, wParam, lParam);
	}
}
