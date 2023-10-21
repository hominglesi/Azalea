using System;
using System.Reflection;

namespace Azalea.Amends;
public abstract class TimedPropertyAmend<T1, T2> : TimedAmend<T1>
{
	private readonly PropertyInfo _property;
	private readonly bool _relative;

	protected T2 AmendValue;

	protected T2? StartingValue;
	protected T2? TargetValue;

	public TimedPropertyAmend(T1 target, string propertyName, T2 value, float duration, bool relative)
		: base(target, null, duration)
	{
		if (target is null) throw new ArgumentNullException(nameof(target));

		var property = target.GetType().GetProperty(propertyName);

		if (property is null || property.PropertyType.Name != typeof(T2).Name)
			throw new ArgumentException($"Invalid property name ({propertyName})");

		_property = property;
		_relative = relative;
		AmendValue = value;
	}

	public override void Start()
	{
		base.Start();
		SetStartAndTargetValue(AmendValue, getPropertyValue(), _relative);
	}

	public override void Finish()
	{
		base.Finish();
		SetPropertyValue(TargetValue!);
	}

	protected abstract void SetStartAndTargetValue(T2 value, T2 currentValue, bool relative);

	private T2 getPropertyValue() => (T2)_property.GetValue(Target)!;
	protected void SetPropertyValue(T2 value) => _property.SetValue(Target, value);
}
