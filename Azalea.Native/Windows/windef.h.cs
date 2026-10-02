namespace Azalea.Native.Windows;
public static partial class Win32
{
	public struct POINT
	{
		public int x;
		public int y;

		#region Utilities

		public static bool operator ==(POINT left, POINT right) => left.Equals(right);
		public static bool operator !=(POINT left, POINT right) => !left.Equals(right);

		public readonly bool Equals(POINT other) => x.Equals(other.x) && y.Equals(other.y);
		public override readonly bool Equals(object? obj)
			=> obj is not null && obj is POINT p && Equals(p);
		public override readonly int GetHashCode() => HashCode.Combine(x, y);

		#endregion
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
