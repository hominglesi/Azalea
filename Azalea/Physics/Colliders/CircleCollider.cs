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
}
