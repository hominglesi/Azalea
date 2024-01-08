using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Physics.Colliders;
public class CircleCollider : Collider
{
	public float Radius { get; set; }
	public override Vector2[] GetVertices()
	{
		throw new NotImplementedException();
	}
}
