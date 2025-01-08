using System;
using System.Numerics;

namespace Azalea.Simulations.Colliders;
public class CircleColliderOld : ColliderOld
{
	public float Radius { get; set; }
	public override float ShortestDistance => Radius;

	public override Vector2[] GetVertices()
	{
		throw new NotImplementedException();
	}

	public override bool ProcessCollision(ColliderOld other, bool resolveCollision)
		=> other.ProcessCollision(this, resolveCollision);

	public override bool ProcessCollision(CircleColliderOld other, bool resolveCollision)
		=> CollisionLogicOld.CircleCircleCollision(this, other, resolveCollision);

	public override bool ProcessCollision(RectColliderOld other, bool resolveCollision)
		=> CollisionLogicOld.CircleRectCollision(this, other, resolveCollision);
}
