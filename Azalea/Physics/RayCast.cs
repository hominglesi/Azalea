using Azalea.Design.Components;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Physics.Colliders;
using System;
using System.Linq;
using System.Numerics;

namespace Azalea.Physics;
public class RayCast
{
	public static Ray Cast(Ray ray)
	{
		Collider collider;
		GameObject ob = new Box();
		collider = new CircleCollider()
		{
			Radius = 2,
		};
		ob.AddComponent(collider);
		ob.Position = ray.StartPosition;
		float radianAngle = ray.Angle; /// 180 * MathF.PI;
		Vector2 direction = new Vector2(MathF.Cos(radianAngle), MathF.Sin(radianAngle));
		direction = Vector2.Normalize(direction);
		collider.Position += direction * ray.MinimumRange;
		for (int i = ray.MinimumRange; i < ray.Range; i++)
		{
			bool isColliding = ray.PGen.CheckCollisions(collider, ComponentStorage<RigidBody>.GetComponents().Select(x => x.Parent.GetComponent<Collider>()));
			if (isColliding)
			{
				ray.Hit = true;
				ray.Distance = i;
				break;
			}

			collider.Position += direction;
		}
		return ray;
	}

}
