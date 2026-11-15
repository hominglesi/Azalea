namespace Azalea.Scenes.Physics;

public class RigidBody2D : Leaf
{
	public override void Invalidate(Invalidation invalidation, Leaf? source = null)
	{
		invalidation |= Invalidation.Physics;
		base.Invalidate(invalidation, source);
	}
}
