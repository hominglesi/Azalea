using System.Runtime.InteropServices;

namespace Azalea.Native.Windows.Win32;
public static partial class Win32
{
	private const string User32Path = "user32.dll";

	[LibraryImport(User32Path, StringMarshalling = StringMarshalling.Utf16)]
	public static partial ushort RegisterClassExW(ref WNDCLASSEXW windowClass);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassexw">Official Documentation</see></summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct WNDCLASSEXW
	{
		public readonly uint cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>();
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
