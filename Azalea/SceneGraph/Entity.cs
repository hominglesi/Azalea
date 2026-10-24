using System;
using System.Collections.Generic;

namespace Azalea.SceneGraph;

public class Entity
{
	#region Children

	private Entity? _parent;
	public Entity? Parent
	{
		get => _parent;
		set
		{
			if (_parent == value) return;

			if (value != null && _parent != null)
				throw new Exception("Cannot add Entity to multiple parents.");

			_parent = value;
		}
	}

	private List<Entity>? _children;
	public IReadOnlyList<Entity> Children
	{
		get => _children ?? (IReadOnlyList<Entity>)[];
		set
		{
			Clear();
			AddRange(value);
		}
	}

	public Entity Child
	{
		get
		{
			if (_children is null || _children.Count != 1)
				throw new Exception("The 'Child' property can only be used when this Entity has exactly one child");

			return _children[0];
		}
		set
		{
			Clear();
			Add(value);
		}
	}

	protected virtual void OnChildIncluded(Entity child) { }
	public void Add(Entity entity)
	{
		_children ??= [];

		ArgumentNullException.ThrowIfNull(entity);

		if (entity == this)
			throw new InvalidOperationException($"Entity cannot be addet to itself.");

		if (entity.Parent is not null)
			throw new InvalidOperationException($"Entity cannot be addet to multiple parents.");

		entity.Parent = this;

		_children.Add(entity);
		OnChildIncluded(entity);
	}

	public void AddRange(IEnumerable<Entity> entities)
	{
		foreach (var entity in entities)
			Add(entity);
	}

	public void Remove(Entity entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		if (entity.Parent != this)
			throw new Exception("Cannot remove Entity that is not a child.");

		_children!.Remove(entity);

		entity.Parent = null;
	}

	public void RemoveRange(IEnumerable<Entity> entities)
	{
		foreach (var entity in entities)
			Remove(entity);
	}

	public void Clear()
	{
		if (_children is null || _children.Count == 0) return;

		foreach (var child in _children)
			child.Parent = null;

		_children.Clear();
	}

	#endregion
}
