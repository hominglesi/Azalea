using System;

namespace Azalea.Amends;
public class InstantAmend<T> : Amend<T>
{
	public InstantAmend(T target, Action<T>? action)
		: base(target, action) { }

	public override void Update(float _)
	{
		Perform();
		Finish();
	}
}
