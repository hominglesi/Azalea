namespace Azalea.Amends;
public class BreakAmend<T> : Amend<T>, IBreakingAmend
{
	public BreakAmend(T amendable)
		: base(amendable, null)
	{

	}

	public override void Update(float deltaTime) { }
}
