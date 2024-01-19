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

	private WindowStyles _style;

	public Win32Window(string title, Vector2Int preferredClientSize, WindowState state,
		bool visible, bool resizable, bool decorated, bool transparentFramebuffer)
	{
		var programHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		var cursor = WinAPI.LoadCursor(IntPtr.Zero, 32512);
		_windowProcedure = windowProcedure;
		var wndClass = new WindowClass("Azalea Window", programHandle, _windowProcedure)
		{
			Style = ClassStyles.OwnDC,
			Cursor = cursor
		};

		WinRectangle windowRect = new(100, 100, preferredClientSize.X, preferredClientSize.Y);
		_style = WindowStyles.Caption | WindowStyles.SysMenu | WindowStyles.MinimizeBox | WindowStyles.SizeBox;

		if (visible) _style |= WindowStyles.Visible;

		WinAPI.AdjustWindowRect(ref windowRect, _style, false, 0);

		var id = WinAPI.RegisterClass(ref wndClass);
		_handle = WinAPI.CreateWindow(
			0,
			id,
			title,
			_style,
			windowRect.X,
			windowRect.Y,
			windowRect.Width,
			windowRect.Height,
			IntPtr.Zero,
			IntPtr.Zero,
			programHandle,
			IntPtr.Zero);

		if (_handle == IntPtr.Zero)
		{
			Console.WriteLine($"Could not create Window. (Error {Marshal.GetLastWin32Error()})");
			return;
		}

		if (resizable == false)
		{
			_style &= ~WindowStyles.SizeBox;
			WinAPI.SetWindowStyle(_handle, _style);
		}

		_deviceContext = WinAPI.GetDC(_handle);

		var pfDescriptor = new PixelFormatDescriptor();
		var format = WinAPI.ChoosePixelFormat(_deviceContext, ref pfDescriptor);

		WinAPI.SetPixelFormat(_deviceContext, format, ref pfDescriptor);

		var glContext = GL.CreateContext(_deviceContext);
		GL.MakeCurrent(_deviceContext, glContext);

		_title = title;
		_visible = visible;
		_resizable = resizable;
	}

	private readonly WindowProcedure _windowProcedure;
	private IntPtr windowProcedure(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
	{
		switch ((WindowMessage)message)
		{
			case WindowMessage.Close:
				Close();
				return IntPtr.Zero;
			case WindowMessage.Move:
				_position = BitwiseUtils.SplitValue(lParam);
				break;
			case WindowMessage.Size:
				_clientSize = BitwiseUtils.SplitValue(lParam);
				break;
			//Mouse Input
			case WindowMessage.LeftButtonDown:
				Input.HandleMouseButtonStateChange(MouseButton.Left, true); break;
			case WindowMessage.LeftButtonUp:
				Input.HandleMouseButtonStateChange(MouseButton.Left, false); break;
			case WindowMessage.RightButtonDown:
				Input.HandleMouseButtonStateChange(MouseButton.Right, true); break;
			case WindowMessage.RightButtonUp:
				Input.HandleMouseButtonStateChange(MouseButton.Right, false); break;
			case WindowMessage.MiddleButtonDown:
				Input.HandleMouseButtonStateChange(MouseButton.Middle, true); break;
			case WindowMessage.MiddleButtonUp:
				Input.HandleMouseButtonStateChange(MouseButton.Middle, false); break;
			case WindowMessage.XButtonDown:
				var xButtonDown = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.HandleMouseButtonStateChange(xButtonDown, true); break;
			case WindowMessage.XButtonUp:
				var xButtonUp = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.HandleMouseButtonStateChange(xButtonUp, false); break;
			case WindowMessage.MouseWheel:
				var delta = BitwiseUtils.GetHighOrderValue(wParam) / 120;
				Input.HandleScroll(delta); break;

			//Keyboad Input
			case WindowMessage.Char:
				Input.HandleTextInput((char)wParam); break;
			case WindowMessage.KeyDown:
				var isRepeat = BitwiseUtils.GetSpecificBit(lParam, 31);
				var key = WindowsExtentions.KeycodeToKey((int)wParam);
				if (isRepeat)
					Input.HandleKeyboardKeyRepeat(key);
				else
					Input.HandleKeyboardKeyStateChange(key, true);
				break;
			case WindowMessage.KeyUp:
				Input.HandleKeyboardKeyStateChange(WindowsExtentions.KeycodeToKey((int)wParam), false); break;
		}

		return WinAPI.DefWindowProc(window, message, wParam, lParam);
	}

	private Vector2Int _mousePosition;

	public override void ProcessEvents()
	{
		while (WinAPI.PeekMessage(out Message message, IntPtr.Zero) != 0)
		{
			_ = WinAPI.TranslateMessage(ref message);
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
	protected override void SetClientSizeImplementation(int width, int height)
		=> WinAPI.SetWindowPos(_handle, IntPtr.Zero, 0, 0, width, height, (uint)SetWindowPosFlags.NoMove);
	protected override void SetDecoratedImplementation(bool enabled) { }
	protected override void SetIconImplementation(Image? data)
	{
		if (data is null)
		{
			WinAPI.SetWindowIcons(_handle, IntPtr.Zero);
			return;
		}

		var icon = WinAPI.CreateIconFromImage(_deviceContext, data);
		WinAPI.SetWindowIcons(_handle, icon);
	}
	protected override void SetOpacityImplementation(float opacity) { }
	protected override void SetPositionImplementation(int x, int y)
		=> WinAPI.SetWindowPos(_handle, IntPtr.Zero, x, y, 0, 0, (uint)SetWindowPosFlags.NoSize);
	protected override void SetResizableImplementation(bool enabled)
	{
		var style = getCurrentStyle();
		if (enabled)
		{
			style |= WindowStyles.SizeBox;
			style |= WindowStyles.MaximizeBox;
		}
		else
		{
			style &= ~WindowStyles.SizeBox;
			style &= ~WindowStyles.MaximizeBox;
		}

		if (WinAPI.SetWindowLong(_handle, WindowLongValue.Style, (uint)style) == 0)
			Console.WriteLine("Failed to change resizable property");

		/*var setPosFlags = SetWindowPosFlags.FrameChanged | SetWindowPosFlags.NoMove | SetWindowPosFlags.NoSize
			| SetWindowPosFlags.NoReposition | SetWindowPosFlags.NoZOrder;
		WinAPI.SetWindowPos(_handle, IntPtr.Zero, 0, 0, 0, 0, (uint)setPosFlags);*/

		WinRectangle rect = new(_position.X, _position.Y, _clientWidth, _clientHeight);
		WinAPI.AdjustWindowRect(ref rect, getCurrentStyle(), false, 0);
		var setPosFlags = SetWindowPosFlags.FrameChanged | SetWindowPosFlags.NoMove | SetWindowPosFlags.NoSize
			| SetWindowPosFlags.NoReposition | SetWindowPosFlags.NoZOrder;
		WinAPI.SetWindowPos(_handle, IntPtr.Zero, rect.X, rect.Y, rect.Width, rect.Height, (uint)setPosFlags);
	}
	protected override void SetShouldCloseImplementation(bool shouldClose) { }
	protected override void SetTitleImplementation(string title)
		=> WinAPI.SetWindowText(_handle, title);
	protected override void SetVisibleImplementation(bool visible)
		=> WinAPI.ShowWindow(_handle, visible ? ShowWindowCommand.Show : ShowWindowCommand.Hide);
	protected override void SetVSyncImplementation(bool enabled) { }
	protected override void SwapBuffersImplementation() => WinAPI.SwapBuffers(_deviceContext);

	#endregion

	private WindowStyles getCurrentStyle() => (WindowStyles)WinAPI.GetWindowLong(_handle, (int)WindowLongValue.Style);

	protected override void OnDispose()
	{

	}
}
