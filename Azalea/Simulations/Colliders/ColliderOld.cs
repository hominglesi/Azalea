using Azalea.Design.Components;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Simulations.Colliders;
public abstract class ColliderOld : Component
{
	public Vector2 Position { get => GetPosition(); set => SetPosition(value); }
	internal virtual Vector2 GetPosition() => Parent.Position;
	internal virtual void SetPosition(Vector2 pos) => Parent.Position = pos;

	public Vector2 Scale { get => Parent.Scale; set => Parent.Scale = value; }
	public float Rotation { get => Parent.Rotation; set => Parent.Rotation = value; }
	public bool IsTrigger { get; set; }
	public abstract float ShortestDistance { get; }
	public List<ColliderOld> CollidedWith { get; set; } = new List<ColliderOld>();
	public List<ColliderOld> CollidingWith { get; set; } = new List<ColliderOld>();

	public Action<ColliderOld>? OnCollision;
	public Action<ColliderOld>? OnCollisionEnter;
	public Action<ColliderOld>? OnCollisionExit;
	public void OnCollide(ColliderOld other)
	{
		CollidingWith.Add(other);
		OnCollision?.Invoke(other);
	}

	public void OnExitingCollider(ColliderOld other)
	{
		OnCollisionExit?.Invoke(other);
		other.OnCollisionExit?.Invoke(this);
	}

	public void OnEnteringCollider(ColliderOld other)
	{
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

	public abstract bool ProcessCollision(ColliderOld other, bool resolveCollision);
	public abstract bool ProcessCollision(CircleColliderOld other, bool resolveCollision);
	public abstract bool ProcessCollision(RectColliderOld other, bool resolveCollision);
}
