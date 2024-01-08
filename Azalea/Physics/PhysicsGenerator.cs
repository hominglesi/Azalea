using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Physics.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Physics;
public class PhysicsGenerator
{
	//u slucaju da ima potrebe za neki tip gravity manipulationa, stavio sam ovo da moze da se menja
	public Vector2 GravityConstant { get; set; } = new Vector2(0, 9.81f);

	public bool UsesGravity { get; set; } = true;
	public bool UsesFriction { get; set; } = true;
	public bool UsesAirResistance { get; set; } = true;


	public void Update(List<GameObject> objects)
	{
		if (UsesGravity)
			ApplyGravity(objects.Where(x => x.GetComponent<RigidBody>().UsesGravity).ToList());

		ApplyForces(objects);
	}

	private void ApplyGravity(List<GameObject> objects)
	{
		foreach (GameObject ob in objects)
		{
			RigidBody rb = ob.GetComponent<RigidBody>();
			rb.Force += rb.Mass * GravityConstant / 60;
		}
	}

	private void ApplyForces(List<GameObject> objects)
	{
		foreach (GameObject ob in objects)
		{
			RigidBody rb = ob.GetComponent<RigidBody>();
			if (rb.IsDynamic == false)
				return;

			rb.Velocity += rb.Force / rb.Mass;
			rb.Position += rb.Velocity;

			CheckCollisions(ob, objects);

			rb.Force = new Vector2(0, 0);
		}
	}

	private void CheckCollisions(GameObject currentObject, List<GameObject> objects)
	{
		Collider currentCollider = currentObject.GetComponent<Collider>();
		RigidBody currentRigidBody = currentObject.GetComponent<RigidBody>();

		foreach (GameObject otherObject in objects)
		{
			if (otherObject == currentObject)
				continue;

			Collider otherCollider = otherObject.GetComponent<Collider>();
			RigidBody otherRigidBody = otherObject.GetComponent<RigidBody>();

			if (currentCollider is CircleCollider crCol1 && otherCollider is CircleCollider crCol2)
			{
				CheckCircleOnCircleCollision(crCol1, crCol2);
			}
			else if (currentCollider is CircleCollider crCol3 && otherCollider is RectCollider rectCol1)
			{
				CheckCircleOnRectCollision(crCol3, rectCol1);
			}
			else if (currentCollider is RectCollider rectCol2 && otherCollider is CircleCollider crCol4)
			{
				CheckCircleOnRectCollision(crCol4, rectCol2);
			}
			else if (currentCollider is RectCollider rectCol3 && otherCollider is RectCollider rectCol4)
			{
				CheckRectOnRectCollision(rectCol3, rectCol4);
			}

		}
	}

	private bool CheckCircleOnCircleCollision(CircleCollider circle1,CircleCollider circle2)
	{
		float distanceOfCenters=Vector2.Distance(circle1.Position, circle2.Position);
		if(circle1.Radius+circle2.Radius>=distanceOfCenters)
		{
			float penetration = circle1.Radius + circle2.Radius - distanceOfCenters;
			ResolveCircleOnCircleCCollision(circle1, circle2, penetration);
			circle1.Parent.Color = Palette.Cyan;
			return true;
		}
		return false;
	}

	private bool CheckCircleOnRectCollision(CircleCollider circle1, RectCollider rect2)
	{
		return false;
	}

	private bool CheckRectOnRectCollision(RectCollider rect1, RectCollider rect2)
	{
		return false;
	}

	private void ResolveCircleOnCircleCCollision(CircleCollider circle1, CircleCollider circle2, float penetration)
	{
		Vector2 collisionNormal = Vector2.Normalize(circle2.Position - circle1.Position);
		circle1.Position -= collisionNormal * (penetration / 2);
		circle2.Position += collisionNormal * (penetration / 2);


	}


}
