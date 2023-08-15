using Azalea.Layout;
using Azalea.Lists;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.Containers;

public partial class CompositeGameObject : GameObject
{
    public CompositeGameObject()
    {
        var childComparer = new ChildComparer(this);

        internalChildren = new SortedList<GameObject>(childComparer);
    }

    #region Parenting

    private readonly SortedList<GameObject> internalChildren;
    protected internal IReadOnlyList<GameObject> InternalChildren => internalChildren;

    private ulong currentChildID;

    protected virtual void AddInternal(GameObject gameObject)
    {
        if (gameObject == null)
            throw new ArgumentNullException(nameof(gameObject), $"Cannot add null {nameof(gameObject)} to {nameof(CompositeGameObject)}.");

        if (gameObject == this)
            throw new InvalidOperationException($"{nameof(CompositeGameObject)} cannot be addet to itself.");

        if (gameObject.ChildID != 0)
            throw new InvalidOperationException($"Cannot add Game Object to multiple containers.");

        gameObject.ChildID = ++currentChildID;
        gameObject.Parent = this;

        internalChildren.Add(gameObject);
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

    protected virtual bool RemoveInternal(GameObject gameObject)
    {
        ArgumentNullException.ThrowIfNull(gameObject);

        int index = IndexOfInternal(gameObject);
        if (index < 0)
            return false;

        internalChildren.RemoveAt(index);

        gameObject.Parent = null;
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
    }

    #endregion

    public override bool UpdateSubTree()
    {
        if (base.UpdateSubTree() == false) return false;

        for (int i = 0; i < internalChildren.Count; ++i)
        {
            internalChildren[i].UpdateSubTree();
        }
        return true;
    }

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

    protected override DrawNode CreateDrawNode() => new CompositeGameObjectDrawNode(this);

    public override DrawNode? GenerateDrawNodeSubtree()
    {
        var drawNode = base.GenerateDrawNodeSubtree();

        if ((drawNode is ICompositeDrawNode compositeNode) == false)
            return null;

        compositeNode.Children ??= new List<DrawNode>();

        int j = 0;
        addFromComposite(ref j, this, compositeNode.Children);

        if (j < compositeNode.Children.Count)
            compositeNode.Children.RemoveRange(j, compositeNode.Children.Count - j);

        return drawNode;
    }

    private static void addFromComposite(ref int j, CompositeGameObject parentComposite, List<DrawNode> target)
    {
        var children = parentComposite.internalChildren;

        for (int i = 0; i < children.Count; i++)
        {
            var drawable = children[i];

            DrawNode? next = drawable.GenerateDrawNodeSubtree();
            if (next == null) continue;

            if (j < target.Count)
                target[j] = next;
            else
                target.Add(next);

            j++;
        }
    }

    private MarginPadding _padding;
    public MarginPadding Padding
    {
        get => _padding;
        protected set
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
