using Azalea.Extentions;
using System;
using System.Numerics;

namespace Azalea.Simulations.Colliders;
internal static class CollisionLogic
{
	public static bool CircleCircleCollision(CircleCollider circle1, CircleCollider circle2, bool resolveCollision)
	{
		float distanceOfCenters = Vector2.Distance(circle1.Position, circle2.Position);

		if (circle1.Radius + circle2.Radius < distanceOfCenters)
			return false;

		if (resolveCollision)
		{
			RigidBody rigidBody1 = circle1.Parent!.GetComponent<RigidBody>()!;
			RigidBody rigidBody2 = circle2.Parent!.GetComponent<RigidBody>()!;
			float displacement = circle1.Radius + circle2.Radius - distanceOfCenters;
			Vector2 collisionNormal = Vector2.Normalize(circle2.Position - circle1.Position);

			//Displacing the balls :D
			if (rigidBody1.IsDynamic)
				circle1.Position -= collisionNormal * (displacement / 2);
			else
				displacement *= 2;

			if (rigidBody2.IsDynamic)
				circle2.Position += collisionNormal * (displacement / 2);

			//Applying newtons lawsssss
			//Vector2 totalForce = rigidBody1.Force + rigidBody2.Force;
			Vector2 relativeVelocity = rigidBody2.Velocity - rigidBody1.Velocity;
			float impulse = 2 * rigidBody1.Mass * rigidBody2.Mass / (rigidBody1.Mass + rigidBody2.Mass)
				* Vector2.Dot(relativeVelocity, collisionNormal) * (rigidBody1.Restitution + rigidBody2.Restitution) / 2;

			// Update velocities based on impulse and mass
			rigidBody1.Velocity += impulse / rigidBody1.Mass * collisionNormal;
			rigidBody2.Velocity -= impulse / rigidBody2.Mass * collisionNormal;
		}

		circle1.OnCollide(circle2);
		circle2.OnCollide(circle1);

		return true;
	}

	public static bool CircleRectCollision(CircleCollider circle, RectCollider rect, bool resolveCollision)
	{
		float rectAngle = rect.Rotation / 180 * MathF.PI;
		int rotatedCircleX = (int)((circle.Position.X - rect.Position.X) * Math.Cos(-rectAngle)
			- (circle.Position.Y - rect.Position.Y) * Math.Sin(-rectAngle) + rect.Position.X);
		int rotatedCircleY = (int)((circle.Position.X - rect.Position.X) * Math.Sin(-rectAngle)
			+ (circle.Position.Y - rect.Position.Y) * Math.Cos(-rectAngle) + rect.Position.Y);

		// Find closest point on the rotated rectangle
		float closestX = Math.Clamp(rotatedCircleX, rect.Position.X - rect.HalfA, rect.Position.X + rect.HalfA);
		float closestY = Math.Clamp(rotatedCircleY, rect.Position.Y - rect.HalfB, rect.Position.Y + rect.HalfB);

		// Calculate distance
		float distanceX = rotatedCircleX - closestX;
		float distanceY = rotatedCircleY - closestY;

		//TODO fix so this can work with triggers p1

		if (distanceX == 0 && distanceY == 0 && rect.IsTrigger == false && circle.IsTrigger == false)
			return false;

		//x2 = cosβx1 − sinβy1
		//y2 = sinβx1 + cosβy1

		double distance = Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

		//TODO fix so this can work with triggers p2
		if (distance >= circle.Radius && (circle.Position - rect.Position).Length() > circle.Radius)
			return false;

		float unrotatedRectPositionX = (float)((closestX - rect.Position.X) * Math.Cos(rectAngle) + (closestY - rect.Position.Y) * Math.Sin(rectAngle) + rect.Position.X);
		float unrotatedRectPositionY = (float)(-(closestX - rect.Position.X) * Math.Sin(rectAngle) + (closestY - rect.Position.Y) * Math.Cos(rectAngle) + rect.Position.Y);

		var rotatedRectPosition = new Vector2(unrotatedRectPositionX, unrotatedRectPositionY);
		float penetration = (float)(circle.Radius - distance);
		if (resolveCollision)
		{
			RigidBody rigidBodyCircle = circle.Parent!.GetComponent<RigidBody>()!;
			RigidBody rigidBodyRect = rect.Parent!.GetComponent<RigidBody>()!;

			Vector2 collisionNormal = Vector2.Normalize(new Vector2(distanceX, distanceY));
			if (float.IsNaN(collisionNormal.X) || float.IsNaN(collisionNormal.Y))
				Console.WriteLine("Collision Normal is NAN");

			collisionNormal = collisionNormal.Rotate(rectAngle, false);

			float displacement = penetration;
			if (float.IsNaN(collisionNormal.X) || float.IsNaN(collisionNormal.Y))
				collisionNormal = new(1, 1);

			if (rigidBodyCircle.IsDynamic)
				circle.Position += collisionNormal * (displacement / 2);
			else
				displacement *= 2;

			if (rigidBodyRect.IsDynamic)
				rect.Position -= collisionNormal * (displacement / 2);

			//Applying newtons lawsssss
			//Vector2 totalForce = rbCircle.Force + rbRect.Force;

			Vector2 relativeVelocity = rigidBodyRect.Velocity - rigidBodyCircle.Velocity;
			float impulse = 2 * rigidBodyCircle.Mass * rigidBodyRect.Mass / (rigidBodyCircle.Mass + rigidBodyRect.Mass)
				* Vector2.Dot(relativeVelocity, collisionNormal) * (rigidBodyCircle.Restitution + rigidBodyRect.Restitution) / 2;

			if (float.IsNaN(impulse))
				impulse = 10;

			/// Calculate impulse in the normal direction
			Vector2 impulseNormal = impulse * collisionNormal;

			// Calculate impulse in the tangential direction (forward)
			Vector2 impulseTangential = relativeVelocity - Vector2.Dot(relativeVelocity, collisionNormal) * collisionNormal;

			// Update velocities based on impulse and mass
			rigidBodyCircle.Velocity += impulseNormal / rigidBodyCircle.Mass;
			rigidBodyCircle.Velocity += impulseTangential / rigidBodyCircle.Mass;
			rigidBodyRect.Velocity -= impulse / rigidBodyRect.Mass * collisionNormal;

			//float angularVelocityRect = Vector2Extentions(radiusRect, -collisionNormal * impulse) / rbRect.MomentOfInertia;
		}

		circle.OnCollide(rect);
		rect.OnCollide(circle);

		return true;
	}

	public static bool RectRectCollision(RectCollider rect1, RectCollider rect2, bool resolveCollision)
	{
		if (rect1.Position.X - rect1.HalfA > rect2.Position.X + rect2.HalfA ||
		   rect1.Position.X + rect1.HalfA < rect2.Position.X - rect2.HalfA ||
		   rect1.Position.Y - rect1.HalfB > rect2.Position.Y + rect2.HalfB ||
		   rect1.Position.Y + rect1.HalfB < rect2.Position.Y - rect2.HalfB)
			return false;

		float penetrationX = Math.Abs(rect1.Position.X - rect2.Position.X) - rect1.HalfA - rect2.HalfA;
		float penetrationY = Math.Abs(rect1.Position.Y - rect2.Position.Y) - rect1.HalfB - rect2.HalfB;

		if (resolveCollision)
		{
			RigidBody rigidBody1 = rect1.Parent!.GetComponent<RigidBody>()!;
			RigidBody rigidBody2 = rect2.Parent!.GetComponent<RigidBody>()!;
			float displacementX = -penetrationX;
			float displacementY = -penetrationY;

			if (penetrationX > penetrationY)
			{
				if (rect1.Position.X <= rect2.Position.X)
					displacementX *= -1;

				if (rigidBody1.IsDynamic)
					rect1.Position += new Vector2(displacementX / 2, 0);
				else
					displacementX *= 2;

				if (rigidBody2.IsDynamic)
					rect2.Position += new Vector2(-1 * displacementX / 2, 0);
			}
			else
			{
				if (rect1.Position.Y <= rect2.Position.Y)
					displacementY *= -1;

				if (rigidBody1.IsDynamic)
					rect1.Position += new Vector2(0, displacementY / 2);
				else
					displacementY *= 2;

				if (rigidBody2.IsDynamic)
					rect2.Position += new Vector2(0, -1 * displacementY / 2);
			}
		}

		rect1.OnCollide(rect2);
		rect2.OnCollide(rect1);

		return true;
	}
}
