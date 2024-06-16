using Azalea.Graphics;
using Azalea.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Azalea.Design.Containers;

public abstract class FlowContainer : Composition
{
	private Dictionary<GameObject, float> _childOrder = new();

	public FlowContainer()
	{
		AddLayout(_layout);
		AddLayout(_childLayout);
	}

	private readonly LayoutValue _layout = new(Invalidation.DrawSize);
	private readonly LayoutValue _childLayout = new(Invalidation.RequiredParentSizeToFit | Invalidation.Presence, InvalidationSource.Child);

	public virtual void InvalidateLayout() => _layout.Invalidate();

	public override void Add(GameObject gameObject)
	{
		_childOrder.Add(gameObject, 0);

		InvalidateLayout();
		base.Add(gameObject);
	}

	public override bool Remove(GameObject gameObject)
	{
		_childOrder.Remove(gameObject);

		InvalidateLayout();
		return base.Remove(gameObject);
	}

	public override void Clear()
	{
		_childOrder.Clear();

		InvalidateLayout();
		base.Clear();
	}

	public void SetChildOrder(GameObject obj, float index)
	{
		if (_childOrder.ContainsKey(obj) == false)
			throw new Exception("Cannot change order of an object that is not a child of this container.");

		_childOrder[obj] = index;
		InvalidateLayout();
	}

	public float GetChildOrder(GameObject obj)
	{
		if (_childOrder.ContainsKey(obj) == false)
			throw new Exception("Cannot get order of an object that is not a child of this container.");

		return _childOrder[obj];
	}

	protected IEnumerable<GameObject> FlexChildren => Children.Where(x => x.IsPresent).OrderBy(x => _childOrder[x]).ThenBy(x => x.ChildID);

	protected abstract IEnumerable<Vector2> ComputeLayoutPositions();

	private void performLayout()
	{
		if (Children.Any() == false)
			return;

		using var positionEnumerator = ComputeLayoutPositions().GetEnumerator();
		using var childEnumerator = FlexChildren.GetEnumerator();

		while (true)
		{
			bool nextPosition = positionEnumerator.MoveNext();
			bool nextChild = childEnumerator.MoveNext();

			if (nextPosition != nextChild)
				throw new Exception($"{nameof(ComputeLayoutPositions)} must return the same amount of positions as FlexChildren");

			if (nextPosition == false)
				return;

			var obj = childEnumerator.Current;
			var pos = positionEnumerator.Current;

			Debug.Assert(obj is not null);

			if (obj.RelativePositionAxes != Axes.None)
				throw new InvalidOperationException($"A flow composition cannot contain a child with relative positioning.");

			obj.Position = pos;
		}
	}

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		if (_childLayout.IsValid == false)
		{
			InvalidateLayout();
			_childLayout.Validate();
		}

		if (_layout.IsValid == false)
		{
			PerformLayout();
		}
	}

	public void PerformLayout()
	{
		performLayout();
		_layout.Validate();
	}

	public void AddNewLine(float newLineSize = 0)
		=> Add(new FlowNewLine(newLineSize));

	public class FlowNewLine : GameObject
	{
		public float Length { get; init; }

		public FlowNewLine(float length = 0)
		{
			Length = length;
		}
	}
}
