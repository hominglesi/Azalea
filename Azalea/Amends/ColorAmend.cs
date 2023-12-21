using Azalea.Graphics.Colors;
using Azalea.Utils;

namespace Azalea.Amends;
public class ColorAmend<T> : TimedPropertyAmend<T, ColorQuad>
{
	public ColorAmend(T target, string propertyName, ColorQuad value, float duration, bool relative)
			: base(target, propertyName, value, duration, relative) { }

	protected override void SetStartAndTargetValue(ColorQuad value, ColorQuad currentValue, bool relative)
	{
		StartingValue = currentValue;

		//if (relative)
		//TargetValue = currentValue + value;
		//else
		TargetValue = value;
	}

	public override void Perform()
	{
		SetPropertyValue(new ColorQuad()
		{
			TopLeft = mapColor(StartingValue.TopLeft, TargetValue.TopLeft),
			TopRight = mapColor(StartingValue.TopRight, TargetValue.TopRight),
			BottomRight = mapColor(StartingValue.BottomRight, TargetValue.BottomRight),
			BottomLeft = mapColor(StartingValue.BottomLeft, TargetValue.BottomLeft),
		});
	}

	private Color mapColor(Color firstColor, Color secondColor)
	{
		var newR = MathUtils.Map(RemainingDuration, StartingDuration, 0, firstColor.RNormalized, secondColor.RNormalized);
		var newG = MathUtils.Map(RemainingDuration, StartingDuration, 0, firstColor.GNormalized, secondColor.GNormalized);
		var newB = MathUtils.Map(RemainingDuration, StartingDuration, 0, firstColor.BNormalized, secondColor.BNormalized);
		var newA = MathUtils.Map(RemainingDuration, StartingDuration, 0, firstColor.ANormalized, secondColor.ANormalized);
		return new Color(newR, newG, newB, newA);
	}

}
