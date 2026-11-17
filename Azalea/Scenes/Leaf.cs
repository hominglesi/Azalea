using Azalea.Graphics;
using Azalea.Graphics.Primitives;
using Azalea.Platform.Rendering.Coordination;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace Azalea.Scenes;

public class Leaf : IComparer<Leaf>
{
	#region Transform

	public float X
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Transform);
		}
	}

	public float Y
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Transform);
		}
	}

	public Vector2 Position
	{
		get => new(X, Y);
		set
		{
			if (X == value.X && Y == value.Y) return;
			X = value.X;
			Y = value.Y;
			Invalidate(Invalidation.Transform);
		}
	}

	public float Width
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Transform);
		}
	}

	public float Height
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Transform);
		}
	}

	public Vector2 Size
	{
		get => new(Width, Height);
		set
		{
			if (Width == value.X && Height == value.Y) return;
			Width = value.X;
			Height = value.Y;
			Invalidate(Invalidation.Transform);
		}
	}

	public Axes RelativeSizeAxes
	{
		get;
		set
		{
			if (field == value) return;

			field = value;

			if (field.HasFlag(Axes.X) && Width == 0) Width = 1;
			if (field.HasFlag(Axes.Y) && Height == 0) Height = 1;

			Invalidate(Invalidation.Transform);
		}
	}

	public float Rotation
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Transform);
		}
	}

	public virtual Vector2 GetChildSpace(bool childPerspective = true) => LocalSize;
	public virtual Vector2 ChildOffset => Vector2.Zero;
	public virtual Vector2 ChildScale => Vector2.One;
	internal void RecalculateTransform()
	{
		_drawInfo = Parent?.DrawInfo ?? new DrawInfo(null, null);

		var childOffset = Parent?.ChildOffset;
		if (childOffset is not null && childOffset != Vector2.Zero)
			_drawInfo.ApplyTranslation(childOffset.Value);

		if (Parent is not null && Parent.ChildScale != Vector2.One) _drawInfo.ApplyScale(Parent.ChildScale);

		if (Position != Vector2.Zero) _drawInfo.ApplyTranslation(Position);
		if (Rotation != 0) _drawInfo.ApplyRotation(Rotation);

		_localSize = Size;
		if (RelativeSizeAxes != Axes.None && Parent is not null)
		{
			var parentSize = Parent.GetChildSpace(false);
			parentSize /= Parent.ChildScale;
			if (RelativeSizeAxes.HasFlag(Axes.X)) _localSize.X *= parentSize.X;
			if (RelativeSizeAxes.HasFlag(Axes.Y)) _localSize.Y *= parentSize.Y;
		}

		_worldQuad = new Quad(Vector2.Zero, _localSize) * _drawInfo.Matrix;

		Invalidations &= ~Invalidation.Transform;
	}

	private DrawInfo _drawInfo = new(null, null);
	public DrawInfo DrawInfo
	{
		get
		{
			if (Invalidations.HasFlag(Invalidation.Transform))
				RecalculateTransform();

			return _drawInfo;
		}
	}

	private Quad _worldQuad;
	public Quad WorldQuad
	{
		get
		{
			if (Invalidations.HasFlag(Invalidation.Transform))
				RecalculateTransform();

			return _worldQuad;
		}
	}

	private Vector2 _localSize;
	public Vector2 LocalSize
	{
		get
		{
			if (Invalidations.HasFlag(Invalidation.Transform))
				RecalculateTransform();

			return _localSize;
		}
	}

	#endregion

	public float Depth
	{
		get;
		set
		{
			if (Parent != null)
				throw new InvalidOperationException("Cannot change Depth while being a child.");

			field = value;
		}
	}

	#region Parenting

	public Leaf? Parent
	{
		get;
		internal set
		{
			if (field == value) return;

			field = value;
		}
	}

	internal ulong ChildID { get; private set; }
	private uint _nextChildId = 1;

	private List<Leaf>? _children;
	public IReadOnlyList<Leaf> Children
	{
		get => _children ?? (IReadOnlyList<Leaf>)[];
		set
		{
			Clear();
			AddRange(value);
		}
	}

	public Leaf Child
	{
		get
		{
			if (Children.Count != 1)
				throw new Exception("The 'Child' property can only be used when this Leaf has exactly one child");

			return _children![0];
		}
		set
		{
			Clear();
			Add(value);
		}
	}

	public int Compare(Leaf? x, Leaf? y)
	{
		ArgumentNullException.ThrowIfNull(x);
		ArgumentNullException.ThrowIfNull(y);

		int depthOrder = y.Depth.CompareTo(x.Depth);
		if (depthOrder != 0) return depthOrder;

		return x.ChildID.CompareTo(y.ChildID);
	}

	public int IndexOf(Leaf child)
	{
		if (_children is null || child.Parent != this) return -1;

		var index = _children.IndexOf(child);

		if (index >= 0 && _children[index].ChildID != child.ChildID)
			throw new InvalidOperationException("A non-matching child was returned!");

		return index;
	}

	public void Add(Leaf child)
	{
		ArgumentNullException.ThrowIfNull(child);

		if (child == this)
			throw new InvalidOperationException("Cannot add Leaf to itself.");

		if (child.Parent != null)
			throw new InvalidOperationException("Cannot add Leaf to multiple parents.");

		child.ChildID = _nextChildId++;
		child.Parent = this;

		_children ??= [];
		var childIndex = _children.BinarySearch(child, this);
		if (childIndex < 0) childIndex = ~childIndex;

		_children.Insert(childIndex, child);
		child.Invalidate(Invalidation.Presence | Invalidation.Transform);
	}

	public void AddRange(IEnumerable<Leaf> children)
	{
		foreach (var child in children)
			Add(child);
	}

	public void Remove(Leaf child)
	{
		ArgumentNullException.ThrowIfNull(child);

		var index = IndexOf(child);

		if (index < 0)
			throw new InvalidOperationException("Child is not a part of parent.");

		_children!.RemoveAt(index);
		child.Parent = null;
		Invalidate(Invalidation.Presence, child);
	}

	public void RemoveRange(IEnumerable<Leaf> children)
	{
		foreach (var child in children)
			Remove(child);
	}

	public void Clear()
	{
		if (_children is null) return;

		foreach (var child in _children)
			Remove(child);
	}

	#endregion

	#region Invalidation

	public Invalidation Invalidations { get; internal set; }

	public virtual void Invalidate(Invalidation invalidation, Leaf? source = null)
	{
		if (invalidation.HasFlag(Invalidation.Presence) == false && (Invalidations | invalidation) == Invalidations)
			return;

		source ??= this;

		Invalidations |= invalidation;

		var childInvalidation = invalidation & Invalidation.Transform;
		if (childInvalidation != Invalidation.None)
			foreach (var child in Children)
				child.Invalidate(childInvalidation, source);

		var parentInvalidation = invalidation & (Invalidation.Presence | Invalidation.Physics);
		if (parentInvalidation == Invalidation.None)
			parentInvalidation = Invalidation.Tree;

		Parent?.Invalidate(parentInvalidation, source);
	}

	#endregion

	#region OverrideFlags

	private static ConcurrentDictionary<Type, OverrideFlags>? _overrideDictionary;

	internal static OverrideFlags GetOverrideFlags(Leaf leaf)
	{
		_overrideDictionary ??= [];

		return _overrideDictionary.GetOrAdd(leaf.GetType(), type =>
		{
			var flags = OverrideFlags.None;

			if (type.GetMethod("Validate", BindingFlags.Instance | BindingFlags.Public)?.DeclaringType != typeof(Leaf))
				flags |= OverrideFlags.Draw;

			if (type.GetMethod("Update", BindingFlags.Instance | BindingFlags.Public)?.DeclaringType != typeof(Leaf))
				flags |= OverrideFlags.Update;

			if (type.GetMethod("Draw", BindingFlags.Instance | BindingFlags.Public)?.DeclaringType != typeof(Leaf))
				flags |= OverrideFlags.Draw;

			if (type.GetMethod("DrawAfterChildren", BindingFlags.Instance | BindingFlags.Public)?.DeclaringType != typeof(Leaf))
				flags |= OverrideFlags.DrawAfterChildren;

			return flags;
		});
	}

	public virtual void Validate() { }
	public virtual void Update() { }
	public virtual void FixedUpdate() { }
	public virtual void Draw(RenderCoordinator coordinator) { }
	public virtual void DrawAfterChildren(RenderCoordinator coordinator) { }

	#endregion

	internal enum OverrideFlags
	{
		None = 0,
		Validate = 1,
		Update = 1 << 1,
		FixedUpdate = 1 << 2,
		Draw = 1 << 3,
		DrawAfterChildren = 1 << 4
	}
}

[Flags]
public enum Invalidation
{
	None = 0,
	Tree = 1,
	Presence = 1 << 1,
	Transform = 1 << 2,
	Physics = 1 << 3
}
