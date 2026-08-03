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

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-bringwindowtotop">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool BringWindowToTop(nint hWnd);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-clienttoscreen">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ClientToScreen(nint hWnd, ref POINT lpPoint);

	/// <summary> Helper method to simplify creating icons. </summary>
	public static IntPtr CreateIconFromPixelArray(IntPtr deviceContext, int width, int height, byte[] data)
	{
		//Windows expects BGRA pixels so we have to swap them
		var swappedBuffer = new byte[data.Length];

		for (int i = 0; i < data.Length; i += 4)
		{
			swappedBuffer[i] = data[i + 2];
			swappedBuffer[i + 1] = data[i + 1];
			swappedBuffer[i + 2] = data[i];
			swappedBuffer[i + 3] = data[i + 3];
		}

		nint color = CreateBitmap(width, height, 1, 32, ref swappedBuffer[0]);
		if (color == nint.Zero)
		{
			Console.WriteLine("Failed to create bitmap");
			return nint.Zero;
		}

		nint mask = CreateCompatibleBitmap(deviceContext, width, height);
		if (mask == nint.Zero)
		{
			Console.WriteLine("Failed to create mask");
			DeleteObject(color);
			return nint.Zero;
		}

		var iconInfo = new ICONINFO(mask, color);

		var hIcon = CreateIconIndirect(ref iconInfo);
		if (mask == nint.Zero)
			Console.WriteLine("Failed to create icon");

		DeleteObject(color);
		DeleteObject(mask);

		return hIcon;
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createiconindirect">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial nint CreateIconIndirect(ref ICONINFO piconinfo);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createwindowexw">Official Documentation</see></summary>
	[LibraryImport(User32Path, StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
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

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-flashwindow">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool FlashWindow(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool bInvert);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclientrect">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetClientRect(nint window, out RECT rect);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdc">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial nint GetDC(nint hWnd);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmonitorinfow">Official Documentation</see></summary>
	[LibraryImport(User32Path, EntryPoint = "GetMonitorInfoW", StringMarshalling = StringMarshalling.Utf16)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetMonitorInfoW(IntPtr monitor, ref MonitorInfo info);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowplacement">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetWindowPlacement(nint hWnd, ref WINDOWPLACEMENT lpwndpl);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetWindowRect(nint hWnd, out RECT lpRect);

	[StructLayout(LayoutKind.Sequential)]
	public readonly struct ICONINFO
	{
		// We have to use int instead of bool since C# doesn't want to marshal bool
		private readonly int fIcon = 1;
		public readonly uint xHotspot = 0;
		public readonly uint yHotspot = 0;
		public readonly nint hbmMask;
		public readonly nint hbmColor;

		public ICONINFO(nint mask, nint color)
		{
			hbmMask = mask;
			hbmColor = color;
		}
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-isiconic">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool IsIconic(nint hwnd);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfromwindow">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial nint MonitorFromWindow(nint hwnd, MonitorFromWindowFlags dwFlags);

	[Flags]
	public enum MonitorFromWindowFlags : uint
	{
		DEFAULTTONULL = 0x00000000,
		DEFAULTTOPRIMARY = 0x00000001,
		DEFAULTTONEAREST = 0x00000002,
	}

	[StructLayout(LayoutKind.Sequential)]
	public readonly struct MonitorInfo
	{
		private readonly uint cbSize;
		public readonly RECT rcMonitor;
		public readonly RECT rcWork;
		public readonly uint dwFlags;

		public MonitorInfo()
		{
			cbSize = (uint)Marshal.SizeOf<MonitorInfo>();
			rcMonitor = new RECT();
			rcWork = new RECT();
			dwFlags = 0;
		}
	}

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

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessage">Official Documentation</see></summary>
	[LibraryImport(User32Path, StringMarshalling = StringMarshalling.Utf16)]
	public static partial nint SendMessageW(nint window, WindowMessage message, nint wParam, nint lParam);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setfocus">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial nint SetFocus(nint hWnd);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SetForegroundWindow(nint window);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptrw">Official Documentation</see></summary>
	[LibraryImport(User32Path, StringMarshalling = StringMarshalling.Utf16)]
	public static partial nint SetWindowLongPtrW(nint hWnd, WindowLongValue nIndex, nint dwNewLong);

	public enum WindowLongValue : int
	{
		ExStyle = -20,
		HInstance = -6,
		HWndParent = -8,
		Id = -12,
		Style = -16,
		UserData = -21,
		WndProc = -4
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowplacement">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SetWindowPlacement(nint hWnd, in WINDOWPLACEMENT lpwndpl);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X,
		int Y, int cx, int cy, SetWindowPosFlags uFlags);

	[Flags]
	public enum SetWindowPosFlags : uint
	{
		ASYNCWINDOWPOS = 0x4000,
		DEFERERASE = 0x2000,
		DRAWFRAME = 0x0020,
		FRAMECHANGED = DRAWFRAME,
		HIDEWINDOW = 0x0080,
		NOACTIVATE = 0x0010,
		NOCOPYBITS = 0x0100,
		NOMOVE = 0x0002,
		NOOWNERZORDER = 0x0200,
		NOREDRAW = 0x0008,
		NOREPOSITION = 0x0200,
		NOSENDCHANGING = 0x0400,
		NOSIZE = 0x0001,
		NOZORDER = 0x0004,
		SHOWWINDOW = 0x0040
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowtextw">Official Documentation</see></summary>
	[LibraryImport(User32Path, StringMarshalling = StringMarshalling.Utf16)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SetWindowTextW(nint hWnd, string lpString);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showcursor">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	public static partial int ShowCursor([MarshalAs(UnmanagedType.Bool)] bool show);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow">Official Documentation</see></summary>
	[LibraryImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ShowWindow(nint hWnd, ShowWindowCommand nCmdShow);

	public enum ShowWindowCommand : uint
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
		CREATE = 1,
		MOVE = 3,
		SIZE = 5,
		PAINT = 15,
		CLOSE = 16,
		ERASEBKGND = 20,
		SHOWWINDOW = 24,
		SETCURSOR = 32,
		GETMINMAXINFO = 36,
		WINDOWPOSCHANGING = 70,
		WINDOWPOSCHANGED = 71,
		STYLECHANGING = 124,
		STYLECHANGED = 125,
		SETICON = 128,
		NCCREATE = 129,
		NCCALCSIZE = 131,
		NCHITTEST = 132,
		NCPAINT = 133,
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
		DWMNCRENDERINGCHANGED = 799,

		// Azalea Defined Messages
		AZ_TRAYICON = 1025
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-windowplacement">Official Documentation</see></summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT
	{
		public readonly uint length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>();
		public uint flags;
		public ShowWindowCommand showCmd;
		public POINT ptMinPosition;
		public POINT ptMaxPosition;
		public RECT rcNormalPosition;
		public RECT rcDevice;

		public WINDOWPLACEMENT()
		{

		}
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-windowpos">Official Documentation</see></summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct WINDOWPOS
	{
		public readonly nint hwnd;
		public readonly nint hwndInsertAfter;
		public readonly int x;
		public readonly int y;
		public readonly int cx;
		public readonly int cy;
		public readonly SetWindowPosFlags flags;
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
