using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Containers;

public class Container : Container<GameObject>
{

}

public class Container<T> : CompositeGameObject
	where T : GameObject
{
	private int enumeratorVersion;

	public Container()
	{
		if (typeof(T) == typeof(GameObject))
			internalChildrenAsT = (IReadOnlyList<T>)InternalChildren;
		else
			throw new NotImplementedException("Other types are not currently supported");
	}

	/// <summary>
	/// Accesses the <paramref name="index"/>-th child.
	/// </summary>
	/// <param name="index">The index of the child to access.</param>
	/// <returns>The <paramref name="index"/>-th child.</returns>
	public T this[int index] => Children[index];

	/// <summary>
	/// The amount of elements in <see cref="Children"/>.
	/// </summary>
	public int Count => Children.Count;

	protected virtual Container<T> Content => this;

	public IReadOnlyList<T> Children
	{
		get => internalChildrenAsT;
		set => ChildrenEnumerable = value;
	}

	public IEnumerable<T> ChildrenEnumerable
	{
		set
		{
			Clear();
			AddRange(value);
		}
	}

	private readonly IReadOnlyList<T> internalChildrenAsT;

	public virtual void Add(T gameObject)
	{
		if (gameObject == Content)
			throw new InvalidOperationException("Content may not be added to itself.");

		ArgumentNullException.ThrowIfNull(gameObject);

		if (Content == this)
			AddInternal(gameObject);
		else
			Content.Add(gameObject);
	}

	public virtual void AddRange(IEnumerable<T> range)
	{
		foreach (T gameObject in range)
			Add(gameObject);
	}

	public virtual bool Remove(T gameObject)
	{
		if (Content != this)
			Content.Remove(gameObject);

		return RemoveInternal(gameObject);
	}

	public void RemoveRange(IEnumerable<T> range)
	{
		if (range == null)
			return;

		foreach (T obj in range)
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

	protected override void AddInternal(GameObject gameObject)
	{
		if (Content == this && gameObject != null && (gameObject is T) == false)
		{
			throw new Exception("Cannot add Game Object");
		}

		enumeratorVersion++;

		base.AddInternal(gameObject);
	}

	public void ChangeChildDepth(T child, float newDepth)
	{
		ChangeInternalChildDepth(child, Depth);
	}
}

