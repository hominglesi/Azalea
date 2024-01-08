using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Physics.Colliders;
public class TriangleCollider : Collider
{

	public override Vector2[] GetVertices()
	{
		// For a circle, return a single point representing the center
		return new Vector2[] { Position };
	}
}
