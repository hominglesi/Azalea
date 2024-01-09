using Azalea.Design.Components;
using Azalea.Extentions;
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

			rb.Acceleration = rb.Force / rb.Mass;
			rb.Velocity += rb.Acceleration;
			rb.Position += rb.Velocity;

	//		rb.AngularAccelaration 
			rb.AngularVelocity += rb.AngularAcceleration;
			rb.Rotation += rb.AngularVelocity;


			CheckCollisions(ob, objects);

			rb.Torque = new Vector2(0, 0);
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

	private bool CheckCircleOnRectCollision(CircleCollider circle, RectCollider rect)
	{
		float rectAngle = rect.Rotation / 180 * MathF.PI;
		int rotatedCircleX = (int)((circle.Position.X - rect.Position.X) * Math.Cos(-rectAngle) - (circle.Position.Y - rect.Position.Y) * Math.Sin(-rectAngle) + rect.Position.X);
		int rotatedCircleY = (int)((circle.Position.X - rect.Position.X) * Math.Sin(-rectAngle) + (circle.Position.Y - rect.Position.Y) * Math.Cos(-rectAngle) + rect.Position.Y);

		// Find closest point on the rotated rectangle
		float closestX = Math.Clamp(rotatedCircleX, rect.Position.X-rect.SideA/2, rect.Position.X + rect.SideA / 2);
		float closestY = Math.Clamp(rotatedCircleY, rect.Position.Y-rect.SideB/2, rect.Position.Y + rect.SideB / 2);
	
		// Calculate distance
		float distanceX = rotatedCircleX - closestX;
		float distanceY = rotatedCircleY - closestY;

		Vector2 collisionNormal=Vector2.Normalize(new Vector2(distanceX, distanceY));
		collisionNormal = Vector2Extentions.Rotate(collisionNormal, rectAngle, false);

			//x2 = cosβx1 − sinβy1
			//y2 = sinβx1 + cosβy1

		double distance = Math.Sqrt(Math.Pow(distanceX,2) + Math.Pow(distanceY , 2));

		if(distance <= circle.Radius)
		{
			float unrotatedRectPositionX = (float)((closestX - rect.Position.X) * Math.Cos(rectAngle) + (closestY - rect.Position.Y) * Math.Sin(rectAngle) + rect.Position.X);
			float unrotatedRectPositionY = (float)(-(closestX - rect.Position.X) * Math.Sin(rectAngle) + (closestY - rect.Position.Y) * Math.Cos(rectAngle) + rect.Position.Y);

			Vector2 rotatedRectPosition = new Vector2(unrotatedRectPositionX, unrotatedRectPositionY);
			float penetration = (float)(circle.Radius-distance);
			ResolveCircleOnRectCollision(circle, rect, penetration,collisionNormal,rotatedRectPosition);
			circle.Parent.Color = Palette.Orange;
			rect.Parent.Color = Palette.Orange;
			return true;
		}
		return false;
	}

	private bool CheckRectOnRectCollision(RectCollider rect1, RectCollider rect2)
	{
		return false;
	}

	private void ResolveCircleOnCircleCCollision(CircleCollider circle1, CircleCollider circle2, float penetration)
	{
		//Displacing the balls :D
		Vector2 collisionNormal = Vector2.Normalize(circle2.Position - circle1.Position);
		float displacement = penetration;
		if (circle1.Parent.GetComponent<RigidBody>().IsDynamic)
			circle1.Position -= collisionNormal * (displacement / 2);
		else
			displacement *= 2;
		if (circle2.Parent.GetComponent<RigidBody>().IsDynamic)
			circle2.Position += collisionNormal * (displacement / 2);

		//Applying newtons lawsssss
		RigidBody rbCircle1 = circle1.Parent.GetComponent<RigidBody>();
		RigidBody rbCircle2 = circle2.Parent.GetComponent<RigidBody>();

		Vector2 totalForce = rbCircle1.Force + rbCircle2.Force;

		Vector2 relativeVelocity = rbCircle2.Velocity - rbCircle1.Velocity;
		float impulse = (2 * rbCircle1.Mass * rbCircle2.Mass) / (rbCircle1.Mass + rbCircle2.Mass) * Vector2.Dot(relativeVelocity, collisionNormal) * (rbCircle1.Restitution + rbCircle2.Restitution) / 2;

		// Update velocities based on impulse and mass
		rbCircle1.Velocity += impulse / rbCircle1.Mass * collisionNormal;
		rbCircle2.Velocity -= impulse / rbCircle2.Mass * collisionNormal;
	//	rbCircle1.AddForce(collisionNormal,)
	}

	private void ResolveCircleOnRectCollision(CircleCollider circle, RectCollider rect, float penetration, Vector2 collisionNormal, Vector2 rotatedRectPosition)
	{
		float displacement = penetration;
		if (circle.Parent.GetComponent<RigidBody>().IsDynamic)
			circle.Position += collisionNormal * (displacement / 2);
		else
			displacement *= 2;

		if (rect.Parent.GetComponent<RigidBody>().IsDynamic)
			rect.Position -= collisionNormal * (displacement / 2);

		//Applying newtons lawsssss
		RigidBody rbCircle = circle.Parent.GetComponent<RigidBody>();
		RigidBody rbRect = rect.Parent.GetComponent<RigidBody>();

	//	Vector2 totalForce = rbCircle.Force + rbRect.Force;

		Vector2 relativeVelocity = rbRect.Velocity - rbCircle.Velocity;
		float impulse = (2 * rbCircle.Mass * rbRect.Mass) / (rbCircle.Mass + rbRect.Mass) * Vector2.Dot(relativeVelocity, collisionNormal) * ( rbCircle.Restitution + rbRect.Restitution) / 2;

		// Update velocities based on impulse and mass
		rbCircle.Velocity += impulse / rbCircle.Mass * collisionNormal;
		rbRect.Velocity -= impulse / rbRect.Mass * collisionNormal;
		
		/*Vdd 
		rbRect.Mome
		float angularVelocityRect = Vector2Extentions(radiusRect, -collisionNormal * impulse) / rbRect.MomentOfInertia;*/

	}
}
