using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct GUID
	{
		public readonly int Data1;
		public readonly short Data2;
		public readonly short Data3;
		public readonly byte Data4_1;
		public readonly byte Data4_2;
		public readonly byte Data4_3;
		public readonly byte Data4_4;
		public readonly byte Data4_5;
		public readonly byte Data4_6;
		public readonly byte Data4_7;
		public readonly byte Data4_8;
	}
}
