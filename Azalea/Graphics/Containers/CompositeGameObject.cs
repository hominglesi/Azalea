using Azalea.Lists;
using System;
using System.Collections.Generic;

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
