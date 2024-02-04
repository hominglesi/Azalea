using Azalea.Graphics.Colors;
using Azalea.Physics.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Azalea.Physics;
public class PhysicsGenerator
{
	//u slucaju da ima potrebe za neki tip gravity manipulationa, stavio sam ovo da moze da se menja
	public Vector2 GravityConstant { get; set; } = new(0, 9.81f);
	public static int UpdateRate => 60;
	public bool IsTopDown { get; set; }
	public float StaticFriction { get; set; } = 0.15f;
	public float DynamicFriction { get; set; } = 0.1f;
	public bool DebugMode { get; set; }
	public bool UsesGravity { get; set; } = true;
	public bool UsesFriction { get; set; } = true;
	public bool UsesAirResistance { get; set; } = true;

	private float VelocityStopThreshold = 0.1f;

	public List<RigidBody> RigidBodies { get; set; } = new List<RigidBody>();

	public void Update(IEnumerable<RigidBody> bodies)
	{
		if (DebugMode)
		{
			foreach (var rb in bodies)
			{
				rb.Parent.Color = Palette.Black;
			}
		}
		RigidBodies.Clear();
		RigidBodies.AddRange(bodies);
		foreach (var rb in bodies)
		{
			if (UsesGravity && rb.UsesGravity) applyGravity(rb);
			if (IsTopDown && rb.UsesFriction) applyTopDownFriction(rb);
			applyForces(rb, bodies);
		}

		foreach (var collider in RigidBodies.Select(x => x.Parent.GetComponent<Collider>()))
		{
			collider.CheckColliders();
		}

		foreach (var collider in RigidBodies.Select(x => x.Parent.GetComponent<Collider>()))
		{
			collider.CollidedWith.Clear();
			collider.CollidedWith.AddRange(collider.CollidingWith);
			collider.CollidingWith.Clear();
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
			{
				rb.Force += -(Vector2.Normalize(rb.Velocity) * rb.Mass * GravityConstant.Y / UpdateRate) * StaticFriction;
			}
			else
			{
				rb.Force += -(Vector2.Normalize(rb.Velocity) * rb.Mass * GravityConstant.Y / UpdateRate) * DynamicFriction;
			}
	}

	private void applyForces(RigidBody rb, IEnumerable<RigidBody> others)
	{
		if (rb.IsDynamic == false)
			return;

		rb.Acceleration = rb.Force / rb.Mass;
		rb.Velocity += rb.Acceleration;
		if (rb.Velocity.Length() < VelocityStopThreshold)
			rb.Velocity = new(0, 0);


		/*	if (float.IsNaN(rb.Velocity.X) || float.IsNaN(rb.Velocity.Y))
			{
				rb.Velocity = new(0, 0);
				rb.Position = new(100, 100);
			}*/
		//rb.AngularVelocity += rb.AngularAcceleration;
		//rb.Rotation += rb.AngularVelocity;



		int numOfAttempts = 1 + (int)MathF.Ceiling(rb.Velocity.Length() / rb.Parent.GetComponent<Collider>().ShortestDistance);
		for (int i = 0; i < numOfAttempts; i++)
		{
			rb.Position += rb.Velocity / numOfAttempts;
			CheckCollisions(rb.Parent.GetComponent<Collider>(), others.Select(x => x.Parent.GetComponent<Collider>()), true);
		}
		rb.Torque = new Vector2(0, 0);
		rb.Force = new Vector2(0, 0);
	}

	public bool CheckCollisions(Collider currentCollider, IEnumerable<Collider> colliders, bool shouldResolveCollision = false)
	{
		bool isColliding = false;

		foreach (Collider otherCollider in colliders)
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
