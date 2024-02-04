using Azalea.Graphics;
using Azalea.Utils;

namespace Azalea.Amends;
public class BoundaryAmend<T> : TimedPropertyAmend<T, Boundary>
{
	public BoundaryAmend(T target, string propertyName, Boundary value, float duration, bool relative)
		: base(target, propertyName, value, duration, relative) { }

	protected override void SetStartAndTargetValue(Boundary value, Boundary currentValue, bool relative)
	{
		StartingValue = currentValue;

		if (relative)
			TargetValue = currentValue + value;
		else
			TargetValue = value;
	}

	public override void Perform()
	{
		var newTop = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.Top, TargetValue.Top);
		var newRight = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.Right, TargetValue.Right);
		var newBottom = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.Bottom, TargetValue.Bottom);
		var newLeft = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.Left, TargetValue.Left);
		SetPropertyValue(new Boundary(newTop, newRight, newBottom, newLeft));
	}
}
