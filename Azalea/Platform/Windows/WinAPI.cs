using Azalea.Numerics;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

internal static class WinAPI
{
	private const string User32Path = "user32.dll";
	private const string Gdi32Path = "gdi32.dll";

	[DllImport(User32Path, EntryPoint = "AdjustWindowRectEx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AdjustWindowRect(ref WinRectangle rect, WindowStyles style, bool menu, WindowStylesEx exStyle);

	[DllImport(Gdi32Path, EntryPoint = "ChoosePixelFormat")]
	public static extern int ChoosePixelFormat(IntPtr deviceContext, [In] ref PixelFormatDescriptor descriptor);

	[DllImport(User32Path, EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode)]
	public static extern IntPtr CreateWindow(
		WindowStylesEx stylesEx,
		ushort classAtom,
		string name,
		WindowStyles styles,
		int x,
		int y,
		int width,
		int height,
		IntPtr parent,
		IntPtr menu,
		IntPtr instance,
		IntPtr param);

	[DllImport(User32Path, EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
	public static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

	[DllImport(User32Path, EntryPoint = "DispatchMessageW", CharSet = CharSet.Unicode)]
	public static extern IntPtr DispatchMessage([In] ref Message message);

	[DllImport(User32Path, EntryPoint = "EnableWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnableWindow(IntPtr window, bool enable);

	[DllImport(User32Path, EntryPoint = "GetClientRect")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool getClientRect(IntPtr window, out WinRectangle rect);
	public static RectangleInt GetClientRect(IntPtr window)
	{
		getClientRect(window, out var rect);
		return rect;
	}

	[DllImport(User32Path, EntryPoint = "GetCursorPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCursorPos(out Vector2Int point);

	[DllImport(User32Path, EntryPoint = "GetDC")]
	public static extern IntPtr GetDC(IntPtr window);

	[DllImport(User32Path, EntryPoint = "GetMessageW", CharSet = CharSet.Unicode)]
	private static extern sbyte getMessage(out Message message, IntPtr window, uint wMsgFilterMin, uint wMsgFilterMax);
	public static sbyte GetMessage(out Message message, IntPtr window) => getMessage(out message, window, 0, 0);

	[DllImport(User32Path, EntryPoint = "GetRawInputData")]
	public static extern uint GetRawInputData(IntPtr rawInput, uint command, ref RawInput data, ref uint dataSize, uint headerSize);

	[DllImport(User32Path, EntryPoint = "GetWindowLongW", CharSet = CharSet.Unicode)]
	public static extern int GetWindowLong(IntPtr window, int index);

	[DllImport(User32Path, EntryPoint = "GetWindowRect")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool getWindowRect(IntPtr window, out WinRectangle rect);
	public static RectangleInt GetWindowRect(IntPtr window)
	{
		getWindowRect(window, out var rect);
		return rect;
	}

	[DllImport(User32Path, EntryPoint = "IsDialogMessageW", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool IsDialogMessage(IntPtr window, [In] ref Message message);

	[DllImport(User32Path, EntryPoint = "LoadCursorW", CharSet = CharSet.Unicode)]
	public static extern IntPtr LoadCursor(IntPtr instance, uint cursorValue);

	[DllImport(User32Path, EntryPoint = "PeekMessageW", CharSet = CharSet.Unicode)]
	private static extern sbyte peekMessage(out Message message, IntPtr window, uint minFilter, uint maxFilter, uint remove);
	public static sbyte PeekMessage(out Message message, IntPtr window)
		=> peekMessage(out message, window, 0, 0, 0x0001); // 0x0001 = PM_REMOVE

	[DllImport(User32Path, EntryPoint = "PostQuitMessage")]
	public static extern void PostQuitMessage(int exitCode);

	[DllImport(User32Path, EntryPoint = "RedrawWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RedrawWindow(IntPtr window, WinRectangle? rectangle, IntPtr region, uint flags);

	[DllImport(User32Path, EntryPoint = "RegisterClassExW", CharSet = CharSet.Unicode)]
	public static extern ushort RegisterClass([In] ref WindowClass windowClass);

	[DllImport(User32Path, EntryPoint = "RegisterRawInputDevices", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RegisterRawInputDevices([In] ref RawInputDevice devices, uint count, uint size);

	[DllImport(User32Path, EntryPoint = "ScreenToClient")]
	public static extern bool ScreenToClient(IntPtr window, ref Vector2Int point);

	[DllImport(Gdi32Path, EntryPoint = "SetPixelFormat")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetPixelFormat(IntPtr deviceContext, int format, [In] ref PixelFormatDescriptor descriptor);

	[DllImport(User32Path, EntryPoint = "SetWindowLongW", CharSet = CharSet.Unicode)]
	public static extern uint SetWindowLong(IntPtr window, WindowLongValue index, uint newValue);

	[DllImport(User32Path, EntryPoint = "SetWindowPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(
		IntPtr window,
		IntPtr insertAfter,
		int x,
		int y,
		int width,
		int height,
		uint flags);

	[DllImport(User32Path, EntryPoint = "ShowWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr window, ShowWindowCommand showCommand);

	[DllImport(Gdi32Path, EntryPoint = "SwapBuffers")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SwapBuffers(IntPtr deviceContext);

	[DllImport(User32Path, EntryPoint = "TranslateMessage")]
	public static extern bool TranslateMessage([In] ref Message message);

	[DllImport(User32Path, EntryPoint = "UpdateWindow")]
	public static extern bool UpdateWindow(IntPtr window);

	[DllImport(User32Path, EntryPoint = "WindowFromPoint")]
	public static extern IntPtr WindowFromPoint(Vector2Int point);
}
