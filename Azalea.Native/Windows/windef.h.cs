namespace Azalea.Native.Windows;
public static partial class Win32
{
	public struct POINT
	{
		public int x;
		public int y;
	}

	public struct RECT(int x, int y, int width, int height)
	{
		public int left = x;
		public int top = y;
		public int right = x + width;
		public int bottom = y + height;

		public readonly int X => left;
		public readonly int Y => top;
		public readonly int Width => right - left;
		public readonly int Height => bottom - top;
	}
}
