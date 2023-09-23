namespace Azalea.Graphics.Colors;
public partial struct ColorInfo
{
	public readonly ColorInfo ModifyHue(float value)
	{
		if (HasSingleColor)
		{
			var color = SingleColor;
			color.Hue += value;
			return SolidColor(color);
		}
		else
		{
			var topLeft = TopLeft;
			topLeft.Hue += value;
			var bottomLeft = BottomLeft;
			bottomLeft.Hue += value;
			var bottomRight = BottomRight;
			bottomRight.Hue += value;
			var topRight = TopRight;
			topRight.Hue += value;

			return new ColorInfo(topLeft, bottomLeft, bottomRight, topRight);
		}
	}
	public readonly ColorInfo ModifySaturation(float value)
	{
		if (HasSingleColor)
		{
			var color = SingleColor;
			color.Saturation += value;
			return SolidColor(color);
		}
		else
		{
			var topLeft = TopLeft;
			topLeft.Saturation += value;
			var bottomLeft = BottomLeft;
			bottomLeft.Saturation += value;
			var bottomRight = BottomRight;
			bottomRight.Saturation += value;
			var topRight = TopRight;
			topRight.Saturation += value;

			return new ColorInfo(topLeft, bottomLeft, bottomRight, topRight);
		}
	}

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
