using System;

namespace Azalea.Amends;
public class TimedAmend<T> : Amend<T>
{
	protected float StartingDuration;
	protected float RemainingDuration;

	public TimedAmend(T target, Action<T>? action, float duration)
		: base(target, action)
	{
		StartingDuration = duration;
		RemainingDuration = duration;
	}

	public override void Update(float deltaTime)
	{
		Perform();

		RemainingDuration -= deltaTime;
		if (RemainingDuration <= 0) Finish();
	}
}
