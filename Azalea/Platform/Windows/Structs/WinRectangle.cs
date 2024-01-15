using Azalea.Numerics;

namespace Azalea.Platform.Windows;
internal struct WinRectangle
{
	private int left;
	private int top;
	private int right;
	private int bottom;

	public static implicit operator RectangleInt(WinRectangle rect)
		=> new(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
}
