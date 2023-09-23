namespace Azalea.Graphics.Colors;
public partial struct ColorInfo
{
	public readonly ColorInfo ModifyLuminance(float value)
	{
		if (HasSingleColor)
		{
			var color = SingleColor;
			color.Luminance += value;
			return SolidColor(color);
		}
		else
		{
			var topLeft = TopLeft;
			topLeft.Luminance += value;
			var bottomLeft = BottomLeft;
			bottomLeft.Luminance += value;
			var bottomRight = BottomRight;
			bottomRight.Luminance += value;
			var topRight = TopRight;
			topRight.Luminance += value;

			return new ColorInfo(topLeft, bottomLeft, bottomRight, topRight);
		}
	}
}
