using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Physics.Colliders;
public class RectCollider : Collider
{
	public float SideA { get; set; }
	public float SideB { get; set; }

	public override Vector2[] GetVertices()
	{
		// Vertices in local space
		float halfWidth = SideA / 2;
		float halfHeight = SideB / 2;

		return new Vector2[]
		{
			new Vector2(-halfWidth, -halfHeight),
			new Vector2(halfWidth, -halfHeight),
			new Vector2(halfWidth, halfHeight),
			new Vector2(-halfWidth, halfHeight)
		};
	}
}
