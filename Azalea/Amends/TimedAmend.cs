using System;

namespace Azalea.Amends;

public class TimedAmend<T>(T target, Action<T>? action, float duration)
	: Amend<T>(target, action)
{
	protected float StartingDuration = duration;
	protected float RemainingDuration = duration;

	public override void Update(float deltaTime)
	{
		Perform();

		RemainingDuration -= deltaTime;
		if (RemainingDuration <= 0) Finish();
	}
}
