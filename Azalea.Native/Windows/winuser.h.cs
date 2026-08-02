using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	private const string User32Path = "user32.dll";

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-adjustwindowrectex">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool AdjustWindowRectEx(
		ref RECT lpRect,
		WindowStyles dwStyle,
		[MarshalAs(UnmanagedType.Bool)] bool bMenu,
		WindowStylesExtended dwExStyle);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createwindowexw">Official Documentation</see></summary>
	[DllImport(User32Path, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW", SetLastError = true)]
	public static extern nint CreateWindowExWDLL(
		WindowStylesExtended dwExStyle,
		ushort lpClassName,
		string lpWindowName,
		WindowStyles dwStyle,
		int X,
		int Y,
		int nWidth,
		int nHeight,
		nint hWndParent,
		nint hMenu,
		nint hInstance,
		nint lpParam);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createwindowexw">Official Documentation</see></summary>
	[LibraryImport(User32Path, StringMarshalling = StringMarshalling.Utf16)]
	public static partial nint CreateWindowExW(
		WindowStylesExtended dwExStyle,
		ushort lpClassName,
		string lpWindowName,
		WindowStyles dwStyle,
		int X,
		int Y,
		int nWidth,
		int nHeight,
		nint hWndParent,
		nint hMenu,
		nint hInstance,
		nint lpParam);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-defwindowprocw">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial nint DefWindowProcW(nint hWnd, uint Msg, nint wParam, nint lParam);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-dispatchmessagew">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial nint DispatchMessageW(in MSG message);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdc">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial nint GetDC(nint hWnd);

	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetWindowRect(nint window, out RECT rect);

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MSG
	{
		public nint hwnd;
		public uint message;
		public nuint wParam;
		public nuint lParam;
		public uint time;
		public POINT Point;
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-peekmessagew">Official Documentation</see></summary>
	[LibraryImport(User32Path, EntryPoint = "PeekMessageW")]
	public static partial sbyte PeekMessageW(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerclassexw">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial ushort RegisterClassExW(ref WNDCLASSEXW windowClass);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowtextw">Official Documentation</see></summary>
	[LibraryImport(User32Path, StringMarshalling = StringMarshalling.Utf16)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SetWindowTextW(nint hWnd, string lpString);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ShowWindow(nint hWnd, ShowWindowCommand nCmdShow);

	public enum ShowWindowCommand : int
	{
		HIDE = 0,
		SHOWNORMAL = 1,
		SHOWMINIMIZED = 2,
		SHOWMAXIMIZED = 3,
		SHOWNOACTIVATE = 4,
		SHOW = 5,
		MINIMIZE = 6,
		SHOWMINNOACTIVE = 7,
		SHOWNA = 8,
		RESTORE = 9,
		SHOWDEFAULT = 10,
		FORCEMINIMIZE = 11
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-translatemessage">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool TranslateMessage(in MSG lpMsg);

	public enum WindowMessage : uint
	{
		MOVE = 3,
		SIZE = 5,
		PAINT = 15,
		CLOSE = 16,
		ERASEBKGND = 20,
		SHOWWINDOW = 24,
		SETCURSOR = 32,
		WINDOWPOSCHANGING = 70,
		SETICON = 128,
		NCCALCSIZE = 131,
		NCHITTEST = 132,
		SYNCPAINT = 136,
		NCLBUTTONUP = 162,
		INPUT_DEVICE_CHANGE = 254,
		INPUT = 255,
		KEYDOWN = 256,
		KEYUP = 257,
		CHAR = 258,
		SYSKEYDOWN = 260,
		SYSKEYUP = 261,
		SYSCOMMAND = 274,
		MOUSEMOVE = 512,
		LBUTTONDOWN = 513,
		LBUTTONUP = 514,
		LBUTTONDBLCLK = 515,
		RBUTTONDOWN = 516,
		RBUTTONUP = 517,
		RBUTTONDBLCLK = 518,
		MBUTTONDOWN = 519,
		MBUTTONUP = 520,
		MBUTTONDBLCLK = 521,
		MOUSEWHEEL = 522,
		XBUTTONDOWN = 523,
		XBUTTONUP = 524,
		XBUTTONDBLCLK = 525,
		DROPFILES = 563,

		// Azalea Defined Messages
		AZ_TRAYICON = 1025
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/window-styles">Official Documentation</see></summary>
	[Flags]
	public enum WindowStyles : uint
	{
		BORDER = 0x00800000,
		CAPTION = 0x00C00000,
		CHILD = 0x40000000,
		CHILDWINDOW = CHILD,
		CLIPCHILDREN = 0x02000000,
		CLIPSIBLINGS = 0x04000000,
		DISABLED = 0x08000000,
		DLGFRAME = 0x00400000,
		GROUP = 0x00020000,
		HSCROLL = 0x00100000,
		ICONIC = 0x20000000,
		MAXIMIZE = 0x01000000,
		MAXIMIZEBOX = 0x00010000,
		MINIMIZE = ICONIC,
		MINIMIZEBOX = GROUP,
		OVERLAPPED = 0x00000000,
		OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
		POPUP = 0x80000000,
		POPUPWINDOW = POPUP | BORDER | SYSMENU,
		SIZEBOX = 0x00040000,
		SYSMENU = 0x00080000,
		TABSTOP = GROUP,
		THICKFRAME = SIZEBOX,
		TILED = OVERLAPPED,
		TILEDWINDOW = OVERLAPPEDWINDOW,
		VISIBLE = 0x10000000,
		VSCROLL = 0x00200000
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles">Official Documentation</see></summary>
	[Flags]
	public enum WindowStylesExtended : uint
	{
		ACCEPTFILES = 0x00000010,
		APPWINDOW = 0x00040000,
		CLIENTEDGE = 0x00000200,
		COMPOSITED = 0x02000000,
		CONTEXTHELP = 0x00000400,
		CONTROLPARENT = 0x00010000,
		DLGMODALFRAME = 0x00000001,
		LAYERED = 0x00080000,
		LAYOUTRTL = 0x00400000,
		LEFT = 0x00000000,
		LEFTSCROLLBAR = 0x00004000,
		LTRREADING = LEFT,
		MDICHILD = 0x00000040,
		NOACTIVATE = 0x08000000,
		NOINHERITLAYOUT = 0x00100000,
		NOPARENTNOTIFY = 0x00000004,
		NOREDIRECTIONBITMAP = 0x00200000,
		OVERLAPPEDWINDOW = WINDOWEDGE | CLIENTEDGE,
		PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST,
		RIGHT = 0x00001000,
		RIGHTSCROLLBAR = LEFT,
		RTLREADING = 0x00002000,
		STATICEDGE = 0x00020000,
		TOOLWINDOW = 0x00000080,
		TOPMOST = 0x00000008,
		TRANSPARENT = 0x00000020,
		WINDOWEDGE = 0x00000100
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassexw">Official Documentation</see></summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct WNDCLASSEXW
	{
		public readonly uint cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>();
		[Flags]
		public enum ClassStyles : uint
		{
			BYTEALIGNCLIENT = 0x1000,
			BYTEALIGNWINDOW = 0x2000,
			CLASSDC = 0x0040,
			DBLCLKS = 0x0008,
			DROPSHADOW = 0x00020000,
			GLOBALCLASS = 0x4000,
			HREDRAW = 0x0002,
			NOCLOSE = 0x0200,
			OWNDC = 0x0020,
			PARENTDC = 0x0080,
			SAVEBITS = 0x0800,
			VREDRAW = 0x0001
		}
		public ClassStyles style;
		public nint lpfnWndProc;
		public int cbClsExtra;
		public int cbWndExtra;
		public nint hInstance;
		public nint hIcon;
		public nint hCursor;
		public nint hbrBackground;
		public nint lpszMenuName;
		public nint lpszClassName;
		public nint hIconSm;
		public WNDCLASSEXW() { }
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-wndproc">Official Documentation</see></summary>
	public delegate nint WNDPROC(nint hWnd, uint uMsg, nint wParam, nint lParam);
}
