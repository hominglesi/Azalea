using System;

namespace Azalea.Amends;
public class InstantAmend<T> : Amend<T>
{
	private bool _performed = false;

	public InstantAmend(T target, Action<T>? action)
		: base(target, action) { }

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
