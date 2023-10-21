using Azalea.Utils;
using System.Numerics;

namespace Azalea.Amends;
public class Vector2Amend<T> : TimedPropertyAmend<T, Vector2>
{
	public Vector2Amend(T target, string propertyName, Vector2 value, float duration, bool relative)
		: base(target, propertyName, value, duration, relative) { }

	protected override void SetStartAndTargetValue(Vector2 value, Vector2 currentValue, bool relative)
	{
		StartingValue = currentValue;

		if (relative)
			TargetValue = currentValue + value;
		else
			TargetValue = value;
	}

	public override void Perform()
	{
		var newX = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.X, TargetValue.X);
		var newY = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue.Y, TargetValue.Y);
		SetPropertyValue(new Vector2(newX, newY));
	}
}
