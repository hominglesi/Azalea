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
		var newR = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.SingleColor.RNormalized, TargetValue.SingleColor.RNormalized);
		var newG = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.SingleColor.GNormalized, TargetValue.SingleColor.GNormalized);
		var newB = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.SingleColor.BNormalized, TargetValue.SingleColor.BNormalized);
		var newA = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.SingleColor.ANormalized, TargetValue.SingleColor.ANormalized);
		SetPropertyValue(new Color(newR, newG, newB, newA));
	}
}
