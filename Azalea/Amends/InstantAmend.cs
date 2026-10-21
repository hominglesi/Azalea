using System;

namespace Azalea.Amends;

public class InstantAmend<T>(T target, Action<T>? action)
	: Amend<T>(target, action)
{
	private bool _performed = false;

	public override void Perform()
	{
		base.Perform();
		_performed = true;
	}

	public override void Update(float _)
	{
		Perform();
		Finish();
	}

	public override void Finish()
	{
		if (_performed == false)
			Perform();

		base.Finish();
	}
}
