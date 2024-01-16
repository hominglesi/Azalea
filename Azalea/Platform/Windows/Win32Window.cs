using Azalea.Graphics;
using Azalea.Graphics.OpenGL;
using Azalea.Inputs;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal class Win32Window : PlatformWindow
{
	private readonly IntPtr _handle;
	private readonly IntPtr _deviceContext;

	public Win32Window()
	{
		var programHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;
		var cursor = WinAPI.LoadCursor(IntPtr.Zero, 32512);
		_windowProcedure = windowProcedure;
		var wndClass = new WindowClass("Azalea Window", programHandle, _windowProcedure)
		{
			Style = ClassStyles.OwnDC,
			Cursor = cursor
		};
		var id = WinAPI.RegisterClass(ref wndClass);
		_handle = WinAPI.CreateWindow(
			0,
			id,
			"Azalea Window",
			WindowStyles.OverlappedWindow,
			//WindowStyles.Overlapped | WindowStyles.Caption | WindowStyles.SysMenu
			//| WindowStyles.SizeBox | WindowStyles.MinimizeBox | WindowStyles.MaximizeBox,
			100,
			100,
			1280,
			720,
			IntPtr.Zero,
			IntPtr.Zero,
			programHandle,
			IntPtr.Zero);

		if (_handle == IntPtr.Zero)
		{
			Console.WriteLine($"Could not create Window. (Error {Marshal.GetLastWin32Error()})");
			return;
		}

		_deviceContext = WinAPI.GetDC(_handle);

		var pfDescriptor = new PixelFormatDescriptor();
		var format = WinAPI.ChoosePixelFormat(_deviceContext, ref pfDescriptor);

		WinAPI.SetPixelFormat(_deviceContext, format, ref pfDescriptor);

		var glContext = GL.CreateContext(_deviceContext);
		GL.MakeCurrent(_deviceContext, glContext);
	}

	private readonly WindowProcedure _windowProcedure;
	private IntPtr windowProcedure(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
	{
		switch ((WindowMessage)message)
		{
			case WindowMessage.Close:
				Close();
				return IntPtr.Zero;
			case WindowMessage.Size:
				var size = BitwiseUtils.SplitValue(lParam);

				_clientSize = size;
				return IntPtr.Zero;
		}

		return WinAPI.DefWindowProc(window, message, wParam, lParam);
	}
	/*
	private IntPtr handleNonclientHitTest(IntPtr window, IntPtr lParam, int titleHeight, int resizeBorderSize)
	{
		var rect = WinAPI.GetWindowRect(_handle);
		var pos = BitwiseUtils.SplitValue(lParam) - rect.Position;

		var titleRect = new RectangleInt(0, 0, rect.Width, titleHeight);

		var left = new RectangleInt(0, titleHeight, resizeBorderSize, rect.Height - titleHeight - resizeBorderSize);
		if (left.Contains(pos)) return (IntPtr)10;

		return (IntPtr)1;
	}*/

	private Vector2Int _mousePosition;

	public override void ProcessEvents()
	{
		while (WinAPI.PeekMessage(out Message message, _handle) != 0)
		{
			WinAPI.TranslateMessage(ref message);
			WinAPI.DispatchMessage(ref message);
		}

		//Update mouse position
		WinAPI.GetCursorPos(out _mousePosition);
		WinAPI.ScreenToClient(_handle, ref _mousePosition);
		Input.HandleMousePositionChange(_mousePosition);
	}

	#region Implementations

	protected override void FocusImplementation() { }
	protected override void FullscreenImplementation() { }
	protected override Vector2Int GetFullscreenSize() => new(1920, 1080);
	protected override Vector2Int GetWorkareaSizeImplementation() => new(1920, 1000);
	protected override void MinimizeImplementation() { }
	protected override void RequestAttentionImplementation() { }
	protected override void RestoreFullscreenImplementation(int lastX, int lastY, int lastWidth, int lastHeight) { }
	protected override void SetClientSizeImplementation(int width, int height) { }
	protected override void SetDecoratedImplementation(bool enabled) { }
	protected override void SetIconImplementation(Image? data) { }
	protected override void SetOpacityImplementation(float opacity) { }
	protected override void SetPositionImplementation(int x, int y) { }
	protected override void SetResizableImplementation(bool enabled) { }
	protected override void SetShouldCloseImplementation(bool shouldClose) { }
	protected override void SetTitleImplementation(string title) { }
	protected override void SetVisibleImplementation(bool visible)
		=> WinAPI.ShowWindow(_handle, visible ? ShowWindowCommand.Show : ShowWindowCommand.Hide);
	protected override void SetVSyncImplementation(bool enabled) { }
	protected override void SwapBuffersImplementation() => WinAPI.SwapBuffers(_deviceContext);

	#endregion

	protected override void OnDispose()
	{

	}
}
