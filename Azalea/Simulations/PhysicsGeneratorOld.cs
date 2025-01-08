using Azalea.Design.Components;
using Azalea.Simulations.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Azalea.Simulations;
public class PhysicsGeneratorOld
{
	public const int UpdateRate = 60;

	private const float _velocityStopThreshold = 0.1f;

	public Vector2 GravityConstant { get; set; } = new(0, 9.81f);
	public bool IsTopDown { get; set; }
	public float StaticFriction { get; set; } = 0.15f;
	public float DynamicFriction { get; set; } = 0.1f;
	public bool DebugMode { get; set; }
	public bool UsesGravity { get; set; } = false;
	public bool UsesFriction { get; set; } = true;
	public IEnumerable<RigidBodyOld> RigidBodies => ComponentStorage<RigidBodyOld>.GetComponents();

	internal void Update()
	{
		foreach (var rb in RigidBodies)
		{
			if (UsesGravity && rb.UsesGravity) applyGravity(rb);
			if (IsTopDown && UsesFriction && rb.UsesFriction) applyTopDownFriction(rb);
			applyForces(rb, RigidBodies);
		}

		foreach (var collider in RigidBodies.Select<RigidBodyOld, ColliderOld>(x => x.Parent!.GetComponent<ColliderOld>()!))
		{
			collider.CheckColliders();
			collider.CollidedWith.Clear();
			collider.CollidedWith.AddRange(collider.CollidingWith);
			collider.CollidingWith.Clear();
		}
	}

	private void applyGravity(RigidBodyOld rb)
	{
		rb.Force += rb.Mass * GravityConstant / UpdateRate;
	}

	private void applyTopDownFriction(RigidBodyOld rb)
	{
		if (rb.Velocity.Length() <= 0)
			return;

		if (rb.Velocity == new Vector2(0, 0))
		{
			rb.Force += -(Vector2.Normalize(rb.Velocity) * rb.Mass * GravityConstant.Y / UpdateRate) * StaticFriction;
		}
		else
		{
			rb.Force += -(Vector2.Normalize(rb.Velocity) * rb.Mass * GravityConstant.Y / UpdateRate) * DynamicFriction;
		}
	}

	private void applyForces(RigidBodyOld rb, IEnumerable<RigidBodyOld> others)
	{
		if (rb.IsDynamic == false)
			return;

		rb.Acceleration = rb.Force / rb.Mass;
		rb.Velocity += rb.Acceleration;
		if (rb.Velocity.Length() < _velocityStopThreshold)
			rb.Velocity = new(0, 0);


		/*	if (float.IsNaN(rb.Velocity.X) || float.IsNaN(rb.Velocity.Y))
			{
				rb.Velocity = new(0, 0);
				rb.Position = new(100, 100);
			}*/
		//rb.AngularVelocity += rb.AngularAcceleration;
		//rb.Rotation += rb.AngularVelocity;



		int numOfAttempts = 1 + (int)MathF.Ceiling(rb.Velocity.Length() / rb.Parent.GetComponent<ColliderOld>().ShortestDistance);
		for (int i = 0; i < numOfAttempts; i++)
		{
			rb.Position += rb.Velocity / numOfAttempts;
			CheckCollisions(rb.Parent.GetComponent<ColliderOld>(), others.Select(x => x.Parent.GetComponent<ColliderOld>()), true);
		}
		rb.Torque = new Vector2(0, 0);
		rb.Force = new Vector2(0, 0);
	}

	public bool CheckCollisions(ColliderOld currentCollider, IEnumerable<ColliderOld> colliders, bool shouldResolveCollision = false)
	{
		bool isColliding = false;

		foreach (ColliderOld otherCollider in colliders)
		{
			if (otherCollider == currentCollider)
				continue;

			var resolveCollision = currentCollider.IsTrigger == false && otherCollider.IsTrigger == false && shouldResolveCollision;

			if (currentCollider.ProcessCollision(otherCollider, resolveCollision))
				isColliding = true;
		}
		return isColliding;
	}
}
