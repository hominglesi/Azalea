using System;
using System.Numerics;

namespace Azalea.Physics.Colliders;
public class CircleCollider : Collider
{
	public float Radius { get; set; }
	public override float ShortestDistance => Radius;

	public override Vector2[] GetVertices()
	{
		throw new NotImplementedException();
	}

	public override bool ProcessCollision(Collider other, bool resolveCollision)
		=> other.ProcessCollision(this, resolveCollision);

	public override bool ProcessCollision(CircleCollider other, bool resolveCollision)
		=> CollisionLogic.CircleCircleCollision(this, other, resolveCollision);

	public override bool ProcessCollision(RectCollider other, bool resolveCollision)
		=> CollisionLogic.CircleRectCollision(this, other, resolveCollision);
}
