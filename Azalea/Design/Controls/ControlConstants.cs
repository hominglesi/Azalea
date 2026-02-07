using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;

namespace Azalea.Design.Controls;
public static class ControlConstants
{
	public static float InputControlHeight => 36;

	public static Color DarkTextColor => new(53, 58, 61);
	public static Color DarkControlColor => new(206, 212, 218);

	public static FontUsage NormalFont => FontUsage.Default.With(size: 16);
}
