namespace Azalea.Amends;

public class BreakAmend<T>(T amendable)
	: Amend<T>(amendable, null), IBreakingAmend
{
	public override void Update(float deltaTime) { }
}
