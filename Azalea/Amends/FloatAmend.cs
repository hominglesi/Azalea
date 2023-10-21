using Azalea.Utils;

namespace Azalea.Amends;
public class FloatAmend<T> : TimedPropertyAmend<T, float>
{
	public FloatAmend(T target, string propertyName, float value, float duration, bool relative)
		: base(target, propertyName, value, duration, relative) { }

	protected override void SetStartAndTargetValue(float value, float currentValue, bool relative)
	{
		StartingValue = currentValue;

		if (relative)
			TargetValue = currentValue + value;
		else
			TargetValue = value;
	}

	public override void Perform()
	{
		var newValue = MathUtils.Map(RemainingDuration, StartingDuration, 0, StartingValue, TargetValue);
		SetPropertyValue(newValue);
	}
}
