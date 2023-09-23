namespace Azalea.Graphics.Colors;
public partial class ColorInfo
{
	public void ModifyLuminance(float value)
	{
		if (HasSingleColor)
		{
			var color = SingleColor;
			color.Luminance += value;
			SingleColor = color;
		}
		else
		{
			TopLeft.Luminance += value;
			BottomLeft.Luminance += value;
			BottomRight.Luminance += value;
			TopRight.Luminance += value;
		}
	}
}
