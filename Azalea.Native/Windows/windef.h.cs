namespace Azalea.Native.Windows;
public static partial class Win32
{
	public struct POINT
	{
		public int x;
		public int y;
	}

	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public RECT(int x, int y, int width, int height)
		{
			left = x;
			top = y;
			right = x + width;
			bottom = y + height;
		}

		public readonly int X => left;
		public readonly int Y => top;
		public readonly int Width => right - left;
		public readonly int Height => bottom - top;

		/*
		public WinRectangle(Vector2Int position, Vector2Int size)
			: this(position.X, position.Y, size.X, size.Y) { }
		public readonly Vector2Int Position => new(X, Y);
		public readonly Vector2Int Size => new(Width, Height);

		public static implicit operator RectangleInt(WinRectangle rect)
			=> new(rect._left, rect._top, rect._right - rect._left, rect._bottom - rect._top);

		public static implicit operator WinRectangle(RectangleInt rect)
			=> new(rect.X, rect.Y, rect.Width, rect.Height);*/
	}
}
