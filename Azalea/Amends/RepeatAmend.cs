using System;

namespace Azalea.Amends;
public class RepeatAmend<T> : Amend<T>, IRepeatableAmend
{
	private float _repeatInterval;
	private float _remainingTime;

	public RepeatAmend(T target, Action<T>? action, float repeatInterval)
		: base(target, action)
	{
		_repeatInterval = repeatInterval;
		_remainingTime = 0;
	}

	public override void Update(float deltaTime)
	{
		_remainingTime -= deltaTime;
		if (_remainingTime <= 0)
		{
			_remainingTime = _repeatInterval;
			Perform();
		}
	}
}
