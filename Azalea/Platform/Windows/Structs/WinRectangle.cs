using Azalea.Numerics;

namespace Azalea.Platform.Windows;
internal readonly struct WinRectangle
{
	private readonly int _left;
	private readonly int _top;
	private readonly int _right;
	private readonly int _bottom;

	public WinRectangle(int x, int y, int width, int height)
	{
		_left = x;
		_top = y;
		_right = x + width;
		_bottom = y + height;
	}

	public WinRectangle(Vector2Int position, Vector2Int size)
		: this(position.X, position.Y, size.X, size.Y) { }

	public readonly int Left => _left;
	public readonly int Top => _top;
	public readonly int Right => _right;
	public readonly int Bottom => _bottom;
	public readonly int X => _left;
	public readonly int Y => _top;
	public readonly int Width => _right - _left;
	public readonly int Height => _bottom - _top;
	public readonly Vector2Int Position => new(X, Y);
	public readonly Vector2Int Size => new(Width, Height);

	public static implicit operator RectangleInt(WinRectangle rect)
		=> new(rect._left, rect._top, rect._right - rect._left, rect._bottom - rect._top);

	public static implicit operator WinRectangle(RectangleInt rect)
		=> new(rect.X, rect.Y, rect.Width, rect.Height);
}
