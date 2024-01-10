using Azalea.Extentions;
using Azalea.Graphics.Colors;
using Azalea.Physics.Colliders;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Physics;
public class PhysicsGenerator
{
	//u slucaju da ima potrebe za neki tip gravity manipulationa, stavio sam ovo da moze da se menja
	public Vector2 GravityConstant { get; set; } = new(0, 9.81f);
	public static int UpdateRate => 60;
	public bool IsTopDown { get; set; }
	public float StaticFriction { get; set; } = 0.1f;
	public float DynamicFriction { get; set; } = 0.05f;
	public bool DebugMode { get; set; }

	public bool UsesGravity { get; set; } = true;
	public bool UsesFriction { get; set; } = true;
	public bool UsesAirResistance { get; set; } = true;


	public void Update(IEnumerable<RigidBody> bodies)
	{
		foreach (var rb in bodies)
		{
			if (UsesGravity) applyGravity(rb);
			if (IsTopDown) applyTopDownFriction(rb);
			applyForces(rb, bodies);
		}
	}

	private void applyGravity(RigidBody rb)
	{
		rb.Force += rb.Mass * GravityConstant / UpdateRate;
	}

	private void applyTopDownFriction(RigidBody rb)
	{
		if (rb.Velocity.Length() > 0)
			if (rb.Velocity == new Vector2(0, 0))
				rb.Force += -(Vector2.Normalize(rb.Velocity) * rb.Mass * GravityConstant.Y / UpdateRate) * StaticFriction;
			else
				rb.Force += -(Vector2.Normalize(rb.Velocity) * rb.Mass * GravityConstant.Y / UpdateRate) * DynamicFriction;
	}

	private void applyForces(RigidBody rb, IEnumerable<RigidBody> others)
	{
		if (rb.IsDynamic == false)
			return;

		rb.Acceleration = rb.Force / rb.Mass;
		rb.Velocity += rb.Acceleration;
		if (float.IsNaN(rb.Velocity.X) || float.IsNaN(rb.Velocity.Y))
		{
			rb.Velocity = new(0, 0);
			rb.Position = new(100, 100);
		}

		//		rb.AngularAccelaration 
		rb.AngularVelocity += rb.AngularAcceleration;
		rb.Rotation += rb.AngularVelocity;

		int numOfAttempts = 1 + (int)MathF.Ceiling(rb.Velocity.Length() / rb.Parent.GetComponent<Collider>().ShortestDistance);
		for (int i = 0; i < numOfAttempts; i++)
		{
			rb.Position += rb.Velocity / numOfAttempts;
			CheckCollisions(rb, others);
		}
		rb.Torque = new Vector2(0, 0);
		rb.Force = new Vector2(0, 0);
	}

	private void CheckCollisions(RigidBody rb, IEnumerable<RigidBody> bodies)
	{
		Collider currentCollider = rb.Parent.GetComponent<Collider>();

		foreach (RigidBody otherBody in bodies)
		{
			if (otherBody == rb)
				continue;

			Collider otherCollider = otherBody.Parent.GetComponent<Collider>();

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

	private bool CheckCircleOnCircleCollision(CircleCollider circle1, CircleCollider circle2)
	{
		float distanceOfCenters = Vector2.Distance(circle1.Position, circle2.Position);
		if (circle1.Radius + circle2.Radius >= distanceOfCenters)
		{
			float penetration = circle1.Radius + circle2.Radius - distanceOfCenters;
			ResolveCircleOnCircleCCollision(circle1, circle2, penetration);
			if (DebugMode)
			{
				circle1.Parent.Color = Palette.Cyan;
			}
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
		float closestX = Math.Clamp(rotatedCircleX, rect.Position.X - rect.SideA / 2, rect.Position.X + rect.SideA / 2);
		float closestY = Math.Clamp(rotatedCircleY, rect.Position.Y - rect.SideB / 2, rect.Position.Y + rect.SideB / 2);

		// Calculate distance
		float distanceX = rotatedCircleX - closestX;
		float distanceY = rotatedCircleY - closestY;

		if (distanceX == 0 && distanceY == 0)
			return false;

		Vector2 collisionNormal = Vector2.Normalize(new Vector2(distanceX, distanceY));
		if (float.IsNaN(collisionNormal.X) || float.IsNaN(collisionNormal.Y))
			Console.WriteLine("Collision Normal is NAN");
		collisionNormal = collisionNormal.Rotate(rectAngle, false);

		//x2 = cosβx1 − sinβy1
		//y2 = sinβx1 + cosβy1

		double distance = Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));


		if (distance < circle.Radius)
		{
			float unrotatedRectPositionX = (float)((closestX - rect.Position.X) * Math.Cos(rectAngle) + (closestY - rect.Position.Y) * Math.Sin(rectAngle) + rect.Position.X);
			float unrotatedRectPositionY = (float)(-(closestX - rect.Position.X) * Math.Sin(rectAngle) + (closestY - rect.Position.Y) * Math.Cos(rectAngle) + rect.Position.Y);

			Vector2 rotatedRectPosition = new Vector2(unrotatedRectPositionX, unrotatedRectPositionY);
			float penetration = (float)(circle.Radius - distance);
			ResolveCircleOnRectCollision(circle, rect, penetration, collisionNormal, rotatedRectPosition);
			if (DebugMode)
			{
				circle.Parent.Color = Palette.Orange;
				rect.Parent.Color = Palette.Orange;
			}
			return true;
		}
		if (DebugMode)
		{
			circle.Parent.Color = Palette.Black;
			rect.Parent.Color = Palette.Black;
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
		Console.WriteLine($"Displacement: {displacement}");
		if (float.IsNaN(collisionNormal.X) || float.IsNaN(collisionNormal.Y))
			collisionNormal = new(1, 1);

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
		float impulse = (2 * rbCircle.Mass * rbRect.Mass) / (rbCircle.Mass + rbRect.Mass) * Vector2.Dot(relativeVelocity, collisionNormal) * (rbCircle.Restitution + rbRect.Restitution) / 2;

		if (float.IsNaN(impulse))
			impulse = 10;


		/// Calculate impulse in the normal direction
		Vector2 impulseNormal = impulse * collisionNormal;

		// Calculate impulse in the tangential direction (forward)
		Vector2 impulseTangential = relativeVelocity - Vector2.Dot(relativeVelocity, collisionNormal) * collisionNormal;

		// Update velocities based on impulse and mass
		rbCircle.Velocity += impulseNormal / rbCircle.Mass;
		rbCircle.Velocity += impulseTangential / rbCircle.Mass;
		rbRect.Velocity -= impulse / rbRect.Mass * collisionNormal;



		Console.WriteLine($"Impact: Circle Velocity: {rbCircle.Velocity}");
		Console.WriteLine($"Impact: Rect Velocity: {rbRect.Velocity}");


		//rbCircle.AddForce(collisionNormal, impulse);
		//	rbRect.AddForce(collisionNormal, -impulse);

		/*Vdd 
		rbRect.Mome
		float angularVelocityRect = Vector2Extentions(radiusRect, -collisionNormal * impulse) / rbRect.MomentOfInertia;*/

	}
}
