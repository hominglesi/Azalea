using Azalea.Platform;
using Azalea.Simulations.Colliders;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Simulations;
public static class Physics
{
	private static PhysicsGenerator? _instance;
	public static PhysicsGenerator Instance => _instance ??= GameHost.Main.Physics;

	public static Vector2 GravityConstant
	{
		get => Instance.GravityConstant;
		set => Instance.GravityConstant = value;
	}

	public static bool IsTopDown
	{
		get => Instance.IsTopDown;
		set => Instance.IsTopDown = value;
	}

	public static float StaticFriction
	{
		get => Instance.StaticFriction;
		set => Instance.StaticFriction = value;
	}

	public static float DynamicFriction
	{
		get => Instance.DynamicFriction;
		set => Instance.DynamicFriction = value;
	}

	public static bool DebugMode
	{
		get => Instance.DebugMode;
		set => Instance.DebugMode = value;
	}

	public static bool UsesGravity
	{
		get => Instance.UsesGravity;
		set => Instance.UsesGravity = value;
	}

	public static bool UsesFriction
	{
		get => Instance.UsesFriction;
		set => Instance.UsesFriction = value;
	}

	public static IEnumerable<RigidBody> RigidBodies
		=> Instance.RigidBodies;

	public static bool CheckCollisions(Collider currentCollider, IEnumerable<Collider> colliders, bool shouldResolveCollision = false)
		=> Instance.CheckCollisions(currentCollider, colliders, shouldResolveCollision);
}
