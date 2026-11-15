using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Azalea.Scenes.Physics;

public class PhysicsWorld2D
{
	private List<RigidBody2D>? _rigidBodies;
	private List<PhysicsCollider>? _colliders;

	public void ProcessStep()
	{
		if (_colliders is null) return;

		for (int i = 0; i < _colliders.Count - 1; i++)
		{
			var colliderA = _colliders[i];

			for (int j = i + 1; j < _colliders.Count; j++)
			{
				var colliderB = _colliders[j];

				if (Collisions2D.Collide(colliderA.Collider, colliderB.Collider, out var manifold))
				{
					if (colliderA.Owner is not null)
						colliderA.Owner.Position += -manifold.Normal * manifold.Depth / 2;
					else
						colliderA.Collider.Position += -manifold.Normal * manifold.Depth / 2;

					if (colliderB.Owner is not null)
						colliderB.Owner.Position += manifold.Normal * manifold.Depth / 2;
					else
						colliderB.Collider.Position += manifold.Normal * manifold.Depth / 2;
				}
			}
		}
	}

	public void Add(Leaf leaf, RigidBody2D? owner)
	{
		if (leaf is RigidBody2D rigidBody)
		{
			if (owner is not null)
				throw new InvalidOperationException("A RigidBody2D cannot be a child of another RigidBody2D");

			owner = rigidBody;

			_rigidBodies ??= [];
			_rigidBodies.Add(rigidBody);
		}
		else if (leaf is Collider2D collider)
		{
			_colliders ??= [];
			_colliders.Add(new PhysicsCollider(owner, collider));
		}

		foreach (var child in leaf.Children)
			Add(child, owner);
	}

	public void Remove(Leaf leaf)
	{
		if (leaf is RigidBody2D rigidBody)
		{
			Debug.Assert(_rigidBodies is not null);
			_rigidBodies.Remove(rigidBody);

			if (_colliders is not null)
			{
				int index = 0;
				while (index < _colliders.Count)
				{
					if (_colliders[index].Owner == rigidBody)
						_colliders.RemoveAt(index);
					else
						index++;
				}
			}
		}
		else if (leaf is Collider2D collider)
		{
			Debug.Assert(_colliders is not null);

			int index = 0;
			while (index < _colliders.Count)
			{
				if (_colliders[index].Collider == collider)
					_colliders.RemoveAt(index);
				else
					index++;
			}
		}

		foreach (var child in leaf.Children)
			Remove(child);
	}

	internal static RigidBody2D? FindOwner(Leaf leaf)
	{
		RigidBody2D? owner = null;

		var parent = leaf.Parent;
		while (parent is not null)
		{
			if (parent is RigidBody2D rigidBody)
			{
				if (owner is not null)
					throw new InvalidOperationException("A RigidBody2D cannot be a child of another RigidBody2D");

				owner = rigidBody;
			}

			parent = parent.Parent;
		}

		return owner;
	}

	readonly struct PhysicsCollider(RigidBody2D? owner, Collider2D collider)
	{
		public readonly RigidBody2D? Owner = owner;
		public readonly Collider2D Collider = collider;
	}
}
