using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	private const string Shell32Path = "shell32.dll";

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct NOTIFYICONDATAW
	{
		public int cbSize = Marshal.SizeOf<NOTIFYICONDATAW>();
		public IntPtr hWnd;
		public uint uID;
		public NotifyIconFlag uFlags;
		public WindowMessage uCallbackMessage;
		public IntPtr hIcon;
		private fixed char _szTip[128];
		public uint dwState;
		public uint dwStateMask;
		public fixed char szInfo[256];
		public uint uTimeoutOrVersion;
		public fixed char szInfoTitle[64];
		public uint dwInfoFlags;
		public GUID guidItem;
		public nint hBalloonIcon;

		public NOTIFYICONDATAW() { }

		public string szTip
		{
			set
			{
				ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, 127);

				fixed (char* ptr = _szTip)
				{
					var span = new Span<char>(ptr, 128);
					value.AsSpan().CopyTo(span);
					span[value.Length] = '\0';
				}
			}
		}
	}

	[Flags]
	public enum NotifyIconFlag : uint
	{
		MESSAGE = 0x01,
		ICON = 0x02,
		TIP = 0x04,
		STATE = 0x08,
		INFO = 0x10,
		GUID = 0x20,
		REALTIME = 0x40,
		SHOWTIP = 0x80
	}

	[Flags]
	public enum NotifyIconMessage : uint
	{
		ADD = 0,
		MODIFY = 1,
		DELETE = 2,
		SETFOCUS = 3,
		SETVERSION = 4
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw">Official Documentation</see></summary>
	[LibraryImport(Shell32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool Shell_NotifyIconW(NotifyIconMessage dwMessage, ref NOTIFYICONDATAW lpData);
}
