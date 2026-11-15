namespace Azalea.Scenes.Physics;

public abstract class Collider2D : Leaf
{
	public override void Invalidate(Invalidation invalidation, Leaf? source = null)
	{
		invalidation |= Invalidation.Physics;
		base.Invalidate(invalidation, source);
	}

	internal abstract ColliderShape2D Shape { get; }
}

internal enum ColliderShape2D
{
	Circle,
	Rectangle,
	Count
}
