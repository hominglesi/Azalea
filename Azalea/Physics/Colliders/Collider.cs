using Azalea.Design.Components;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Physics.Colliders;
public abstract class Collider : Component
{
	public Vector2 Position { get => Parent.Position; set => Parent.Position = value; }
	public Vector2 Scale { get => Parent.Scale; set => Parent.Scale = value; }
	public float Rotation { get => Parent.Rotation; set => Parent.Rotation = value; }
	public bool IsTrigger { get; set; }
	public abstract float ShortestDistance { get; }
	public List<Collider> CollidedWith { get; set; } = new List<Collider>();
	public List<Collider> CollidingWith { get; set; } = new List<Collider>();

	public Action<Collider>? OnCollision;
	public Action<Collider>? OnCollisionEnter;
	public Action<Collider>? OnCollisionExit;
	public void OnCollide(Collider other)
	{
		CollidingWith.Add(other);
		OnCollision?.Invoke(other);
	}

	public void OnExitingCollider(Collider other)
	{
		Console.WriteLine("Left trigger");
		OnCollisionExit?.Invoke(other);
		other.OnCollisionExit?.Invoke(this);
	}

	public void OnEnteringCollider(Collider other)
	{
		Console.WriteLine("Entered trigger");
		OnCollisionEnter?.Invoke(other);
		other.OnCollisionEnter?.Invoke(this);
	}

	public void CheckColliders()
	{
		if (IsTrigger == false)
			return;

		//Checking entering collider
		for (int i = 0; i < CollidingWith.Count; i++)
		{
			if (!CollidedWith.Contains(CollidingWith[i]))
			{
				OnEnteringCollider(CollidingWith[i]);
			}
		}

		//Checking exiting collider
		for (int i = 0; i < CollidedWith.Count; i++)
		{
			if (!CollidingWith.Contains(CollidedWith[i]))
			{
				OnExitingCollider(CollidedWith[i]);
			}
		}

	}
	public abstract Vector2[] GetVertices();
}
