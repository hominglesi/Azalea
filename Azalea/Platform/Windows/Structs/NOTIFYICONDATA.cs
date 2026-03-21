using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows.Structs;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NOTIFYICONDATA
{
	public int cbSize;
	public IntPtr hWnd;
	public uint uID;
	public uint uFlags;
	public WindowMessage uCallbackMessage;
	public IntPtr hIcon;
	private fixed char _szTip[128];
	public uint dwState;
	public uint dwStateMask;
	public fixed char szInfo[256];
	public uint uTimeoutOrVersion;
	public fixed char szInfoTitle[64];
	public uint dwInfoFlags;

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
