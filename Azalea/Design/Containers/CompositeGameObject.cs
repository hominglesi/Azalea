using Azalea.Extentions.EnumExtentions;
using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Layout;
using Azalea.Lists;
using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Design.Containers;

public partial class CompositeGameObject : GameObject
{
	public CompositeGameObject()
	{
		var childComparer = new ChildComparer(this);

		internalChildren = new SortedList<GameObject>(childComparer);

		AddLayout(_childrenSizeDependencies);
	}

	private bool _masking;
	public bool Masking
	{
		get => _masking;
		set
		{
			if (_masking == value) return;

			_masking = value;
		}
	}

	internal Boundary MaskingPadding = Boundary.Zero;

	#region Parenting

	private readonly SortedList<GameObject> internalChildren;
	protected internal IReadOnlyList<GameObject> InternalChildren => internalChildren;

	private ulong currentChildID;

	internal virtual void AddInternal(GameObject gameObject)
	{
		if (gameObject == null)
			throw new ArgumentNullException(nameof(gameObject), $"Cannot add null {nameof(gameObject)} to {nameof(CompositeGameObject)}.");

		if (gameObject == this)
			throw new InvalidOperationException($"{nameof(CompositeGameObject)} cannot be addet to itself.");

		if (gameObject.ChildID != 0)
			throw new InvalidOperationException($"Cannot add Game Object to multiple compositions.");

		gameObject.ChildID = ++currentChildID;
		gameObject.Parent = this;

		internalChildren.Add(gameObject);

		Invalidate(Invalidation.Presence, InvalidationSource.Child);

		if (AutoSizeAxes != Axes.None)
			Invalidate(Invalidation.RequiredParentSizeToFit, InvalidationSource.Child);
	}

	protected void AddRangeInternal(IEnumerable<GameObject> objects)
	{
		foreach (GameObject gameObject in objects)
			AddInternal(gameObject);
	}

	protected internal int IndexOfInternal(GameObject gameObject)
	{
		if (gameObject.Parent != null && gameObject.Parent != this)
			throw new InvalidOperationException($@"Cannot call {nameof(IndexOfInternal)} for a drawable that already is a child of a different parent");

		int index = internalChildren.IndexOf(gameObject);

		if (index >= 0 && internalChildren[index].ChildID != gameObject.ChildID)
			throw new InvalidOperationException(@$"A non-matching {nameof(GameObject)} was returned.");

		return index;
	}

	internal virtual bool RemoveInternal(GameObject gameObject)
	{
		ArgumentNullException.ThrowIfNull(gameObject);

		int index = IndexOfInternal(gameObject);
		if (index < 0)
			return false;

		internalChildren.RemoveAt(index);

		gameObject.Parent = null;

		Invalidate(Invalidation.Presence, InvalidationSource.Child);

		if (AutoSizeAxes != Axes.None)
			Invalidate(Invalidation.RequiredParentSizeToFit, InvalidationSource.Child);
		return true;
	}

	protected internal virtual void ClearInternal(bool disposeChildren = true)
	{
		if (internalChildren.Count == 0) return;

		foreach (GameObject t in internalChildren)
		{
			t.Parent = null;
		}

		internalChildren.Clear();

		if (AutoSizeAxes != Axes.None)
			Invalidate(Invalidation.RequiredParentSizeToFit, InvalidationSource.Child);
	}

	protected internal void ChangeInternalChildDepth(GameObject child, float newDepth)
	{
		if (child.Depth == newDepth) return;

		int index = IndexOfInternal(child);
		if (index < 0)
			throw new InvalidOperationException($"Can not change depth of an object which is not contained within this {nameof(CompositeGameObject)}.");

		internalChildren.RemoveAt(index);
		ulong cId = child.ChildID;
		child.ChildID = 0;
		child.Depth = newDepth;
		child.ChildID = cId;

		internalChildren.Add(child);
	}

	#endregion

	protected virtual bool RequiresChildrenUpdate => true;

	public override void UpdateSubTree()
	{
		base.UpdateSubTree();

		for (int i = 0; i < internalChildren.Count; ++i)
		{
			internalChildren[i].UpdateSubTree();
		}

		UpdateAfterChildren();
	}

	public override void FixedUpdateSubTree()
	{
		base.FixedUpdateSubTree();

		for (int i = 0; i < internalChildren.Count; ++i)
		{
			internalChildren[i].FixedUpdateSubTree();
		}
	}

	protected virtual void UpdateAfterChildren() { }

	protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
	{
		bool anyInvalidated = base.OnInvalidate(invalidation, source);

		if (source == InvalidationSource.Child)
			return anyInvalidated;

		invalidation &= ~Invalidation.DrawNode;
		if (invalidation == Invalidation.None)
			return anyInvalidated;

		IReadOnlyList<GameObject> targetChildren = internalChildren;

		for (int i = 0; i < targetChildren.Count; ++i)
		{
			GameObject c = targetChildren[i];

			Invalidation childInvalidation = invalidation;
			if ((invalidation & Invalidation.RequiredParentSizeToFit) > 0)
				childInvalidation |= Invalidation.DrawInfo;

			childInvalidation &= ~Invalidation.MiscGeometry;

			if (c.RelativeSizeAxes == Axes.None)
				childInvalidation &= ~Invalidation.DrawSize;

			anyInvalidated |= c.Invalidate(childInvalidation, InvalidationSource.Parent);
		}

		return base.OnInvalidate(invalidation, source);
	}

	public override void Draw(IRenderer renderer)
	{
		if (Masking)
		{
			var newScissor = (RectangleInt)ScreenSpaceDrawQuad;
			if (MaskingPadding != Boundary.Zero)
			{
				newScissor.X -= MathUtils.Ceiling(MaskingPadding.Left);
				newScissor.Width += MathUtils.Ceiling(MaskingPadding.Horizontal);
				newScissor.Y -= MathUtils.Ceiling(MaskingPadding.Top);
				newScissor.Height += MathUtils.Ceiling(MaskingPadding.Vertical);
			}
			renderer.PushScissor(newScissor);
		}

		foreach (var child in internalChildren)
		{
			child.Draw(renderer);
		}

		if (Masking)
		{
			renderer.PopScissor();
		}
	}

	private Boundary _padding;
	public Boundary Padding
	{
		get => _padding;
		set
		{
			if (_padding.Equals(value)) return;

			_padding = value;

			foreach (GameObject c in internalChildren)
				c.Invalidate(c.InvalidationFromParentSize | Invalidation.MiscGeometry);
		}
	}

	public Vector2 ChildSize => DrawSize - new Vector2(Padding.Horizontal, Padding.Vertical);
	public Vector2 ChildOffset => new(Padding.Left, Padding.Top);

	private Vector2 _relativeChildSize = Vector2.One;
	public Vector2 RelativeChildSize => _relativeChildSize;

	private Vector2 _relativeChildOffset = Vector2.Zero;
	public Vector2 RelativeChildOffset => _relativeChildOffset;

	public Vector2 RelativeToAbsoluteFactor => Vector2.Divide(ChildSize, RelativeChildSize);

	#region AutoSizeAxes

	private Axes _autoSizeAxes;

	public virtual Axes AutoSizeAxes
	{
		get => _autoSizeAxes;
		set
		{
			if (value == _autoSizeAxes) return;

			if ((RelativeSizeAxes & value) != 0)
				throw new InvalidOperationException("No axis can be relatively sized and automatically sized at the same time.");

			_autoSizeAxes = value;

			if (value == Axes.None)
				_childrenSizeDependencies.Validate();
			else
				_childrenSizeDependencies.Invalidate();

			OnSizingChanged();
		}
	}

	public override float Width
	{
		get
		{
			if (_isComputingChildrenSizeDependencies == false && AutoSizeAxes.HasFlagFast(Axes.X))
				updateChildrenSizeDependencies();
			return base.Width;
		}
		set
		{
			if (AutoSizeAxes.HasFlagFast(Axes.X))
				throw new InvalidOperationException("Width of composite cannot be manually set");

			base.Width = value;
		}
	}

	public override float Height
	{
		get
		{
			if (_isComputingChildrenSizeDependencies == false && AutoSizeAxes.HasFlagFast(Axes.Y))
				updateChildrenSizeDependencies();
			return base.Height;
		}
		set
		{
			if (AutoSizeAxes.HasFlagFast(Axes.Y))
				throw new InvalidOperationException("Height of composite cannot be manually set");

			base.Height = value;
		}
	}

	public override Vector2 Size
	{
		get
		{
			if (_isComputingChildrenSizeDependencies == false && AutoSizeAxes != Axes.None)
				updateChildrenSizeDependencies();
			return base.Size;
		}
		set
		{
			if (AutoSizeAxes != Axes.None)
				throw new InvalidOperationException("Size of composite cannot be manually set");

			base.Size = value;
		}
	}

	private bool _isComputingChildrenSizeDependencies;

	private void updateChildrenSizeDependencies()
	{
		_isComputingChildrenSizeDependencies = true;

		try
		{
			if (_childrenSizeDependencies.IsValid == false)
			{
				updateAutoSize();
				_childrenSizeDependencies.Validate();
			}
		}
		finally
		{
			_isComputingChildrenSizeDependencies = false;
		}


	}

	private void updateAutoSize()
	{
		if (AutoSizeAxes == Axes.None) return;

		Vector2 b = computeAutoSize() + Padding.Total;

		base.Size = new Vector2(
			AutoSizeAxes.HasFlagFast(Axes.X) ? b.X : base.Width,
			AutoSizeAxes.HasFlagFast(Axes.Y) ? b.Y : base.Height);
	}

	public Vector2 computeAutoSize()
	{
		Boundary originalPadding = Padding;
		Boundary originalMargin = Margin;

		try
		{
			Padding = new Boundary();
			Margin = new Boundary();

			if (AutoSizeAxes == Axes.None) return DrawSize;

			Vector2 maxBoundSize = Vector2.Zero;

			foreach (var c in InternalChildren)
			{
				Vector2 cBound = c.RequiredParentSizeToFit;

				if (c.IgnoredForAutoSizeAxes.HasFlag(Axes.X) == false)
					maxBoundSize.X = Math.Max(maxBoundSize.X, cBound.X);
				if (c.IgnoredForAutoSizeAxes.HasFlag(Axes.Y) == false)
					maxBoundSize.Y = Math.Max(maxBoundSize.Y, cBound.Y);
			}

			if (AutoSizeAxes.HasFlagFast(Axes.X) == false)
				maxBoundSize.X = DrawSize.X;
			if (AutoSizeAxes.HasFlagFast(Axes.Y) == false)
				maxBoundSize.Y = DrawSize.Y;

			return new Vector2(maxBoundSize.X, maxBoundSize.Y);
		}
		finally
		{
			Padding = originalPadding;
			Margin = originalMargin;
		}
	}

	private readonly LayoutValue _childrenSizeDependencies = new(Invalidation.RequiredParentSizeToFit | Invalidation.Presence, InvalidationSource.Child);

	#endregion

	internal override bool BuildNonPositionalInputQueue(List<GameObject> queue)
	{
		if (base.BuildNonPositionalInputQueue(queue) == false)
			return false;

		foreach (var child in internalChildren)
		{
			child.BuildNonPositionalInputQueue(queue);
		}

		return true;
	}

	internal override bool BuildPositionalInputQueue(Vector2 screenSpacePos, List<GameObject> queue)
	{
		if (base.BuildPositionalInputQueue(screenSpacePos, queue) == false)
			return false;

		foreach (var child in internalChildren)
		{
			child.BuildPositionalInputQueue(screenSpacePos, queue);
		}

		return true;
	}

	#region Invalidation

	internal void InvalidateChildrenSizeDependencies(Invalidation invalidation, Axes axes, GameObject source)
	{
		//bool wasValid = childrenSizeDependencies.IsValid;

		Invalidate(invalidation, InvalidationSource.Child);

		//axes &= ~source.BypassAutoSizedAxes;

		//axes &= AutoSizeAxes;

		//if (wasValid && axes == Axes.None)
		//childrenSizeDependencies.Validate();w
	}

	#endregion

	protected class ChildComparer : IComparer<GameObject>
	{
		private readonly CompositeGameObject _owner;

		public ChildComparer(CompositeGameObject owner)
		{
			_owner = owner;
		}

		public int Compare(GameObject? x, GameObject? y) => _owner.Compare(x ?? throw new Exception("Cannot compare null value"),
																			y ?? throw new Exception("Cannot compare null value"));
	}

	protected virtual int Compare(GameObject x, GameObject y)
	{
		ArgumentNullException.ThrowIfNull(x);
		ArgumentNullException.ThrowIfNull(y);

		int i = y.Depth.CompareTo(x.Depth);
		if (i != 0) return i;

		return x.ChildID.CompareTo(y.ChildID);
	}
}
