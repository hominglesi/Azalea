using System;
using System.Numerics;

namespace Azalea.Physics.Colliders;
public class TriangleCollider : Collider
{
	public override float ShortestDistance => throw new NotImplementedException();

	public override Vector2[] GetVertices()
	{
		// For a circle, return a single point representing the center
		return new Vector2[] { Position };
	}
}
