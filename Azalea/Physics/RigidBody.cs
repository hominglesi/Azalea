using Azalea.Design.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Physics;
public class RigidBody : Component
{
	public Vector2 Position { get { return Parent.Position; } set { Parent.Position = value; } }
	public float Rotation { get { return Parent.Rotation; } set { Parent.Rotation = value; } }
	public Vector2 CenterOfMass { get; set; }
	public float Mass { get; set; } = 1;
	public Vector2 Velocity { get; set; }
	public Vector2 Force { get; set; }
	public Vector2 Torque { get; set; }
	public Vector2 Acceleration { get; set; }
	public float AngularAcceleration { get; set; }
	public bool CanRotate { get; set; } = true;
	public bool UsesGravity { get; set; } = true;
	public bool UsesFriction { get; set; } = true;
	public bool UsesAirResistance { get; set; } = true;
	public float Restitution { get; set; } = 0.9f;
	public float StaticFriction { get; set; } = 1;
	public float DynamicFriction { get; set; } = 1;
	public float AngularVelocity { get; set; }
	public bool IsDynamic { get; set; } = true;
	public void AddForce(Vector2 forceVector, float force)
	{
		Force += forceVector*force;
	}

	public void AddImpulse(Vector2 impulseVector)
	{

	}

}
