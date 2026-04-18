using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;

namespace Azalea.Editor.Design.Gui;
internal static class GUIConstants
{
	public readonly static FontUsage Font = FontUsage.Default.With(size: 14);

	public static class Colors
	{
		public readonly static Color AccentColor = new(32, 50, 76);
		public readonly static Color AccentColor2 = new(61, 133, 224);
	}
}
