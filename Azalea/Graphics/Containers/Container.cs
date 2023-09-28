using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Containers;

public class Container : CompositeGameObject
{
	/// <summary>
	/// Accesses the <paramref name="index"/>-th child.
	/// </summary>
	/// <param name="index">The index of the child to access.</param>
	/// <returns>The <paramref name="index"/>-th child.</returns>
	public GameObject this[int index] => Children[index];

	/// <summary>
	/// The amount of elements in <see cref="Children"/>.
	/// </summary>
	public int Count => Children.Count;

	protected virtual Container Content => this;

	public IReadOnlyList<GameObject> Children
	{
		get
		{
			if (Content == this) return InternalChildren;
			else return Content.InternalChildren;
		}
		set
		{
			Clear();
			AddRange(value);
		}
	}

	public GameObject Child
	{
		set
		{
			Clear();
			Add(value);
		}
	}

	public virtual void Add(GameObject gameObject)
	{
		if (gameObject == Content)
			throw new InvalidOperationException("Content may not be added to itself.");

		ArgumentNullException.ThrowIfNull(gameObject);

		if (Content == this)
			AddInternal(gameObject);
		else
			Content.Add(gameObject);
	}

	public virtual void AddRange(IEnumerable<GameObject> range)
	{
		foreach (GameObject gameObject in range)
			Add(gameObject);
	}

	public virtual bool Remove(GameObject gameObject)
	{
		if (Content != this)
			Content.Remove(gameObject);

		return RemoveInternal(gameObject);
	}

	public void RemoveRange(IEnumerable<GameObject> range)
	{
		if (range == null)
			return;

		foreach (GameObject obj in range)
			Remove(obj);
	}

	public void Clear() => Clear(true);

	public virtual void Clear(bool disposeChildren)
	{
		if (Content != null && Content != this)
			Content.Clear(disposeChildren);
		else
			ClearInternal(disposeChildren);
	}

	public void ChangeChildDepth(GameObject child, float newDepth)
	{
		ChangeInternalChildDepth(child, newDepth);
	}
}

