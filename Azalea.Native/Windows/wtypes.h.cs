using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	[StructLayout(LayoutKind.Sequential)]
	public struct PROPERTYKEY()
	{
		public GUID fmtid;
		public int pid;
	}
}
