using System;

namespace Azalea.Amends;

public class RepeatAmend<T>(T target, Action<T>? action, float repeatInterval)
	: Amend<T>(target, action), IRepeatableAmend
{
	private readonly float _repeatInterval = repeatInterval;
	private float _remainingTime = 0;

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
