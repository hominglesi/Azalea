using Azalea.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Azalea.Graphics.Containers;

/// <summary>
/// A container that can be used to fluently arrange its children.
/// </summary>
public abstract class FlowContainer<T> : Container<T>
    where T : GameObject
{
    internal event Action? OnLayout;

    protected FlowContainer()
    {
        AddLayout(_layout);
        AddLayout(_childLayout);
    }

    private readonly LayoutValue _layout = new(Invalidation.DrawSize);
    private readonly LayoutValue _childLayout = new(Invalidation.RequiredParentSizeToFit | Invalidation.Presence, InvalidationSource.Child);

    protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate || !_layout.IsValid;

    protected virtual void InvalidateLayout() => _layout.Invalidate();

    private readonly Dictionary<GameObject, float> _layoutChildren = new();

    protected override void AddInternal(GameObject gameObject)
    {
        _layoutChildren.Add(gameObject, 0f);

        InvalidateLayout();
        base.AddInternal(gameObject);
    }

    protected override bool RemoveInternal(GameObject gameObject)
    {
        _layoutChildren.Remove(gameObject);

        InvalidateLayout();
        return base.RemoveInternal(gameObject);
    }

    protected internal override void ClearInternal(bool disposeChildren = true)
    {
        _layoutChildren.Clear();

        InvalidateLayout();
        base.ClearInternal(disposeChildren);
    }

    public void SetLayoutPosition(GameObject gameObject, float newPosition)
    {
        if (!_layoutChildren.ContainsKey(gameObject))
            throw new InvalidOperationException($"Cannot change layout position of game object which is not contained within this {nameof(FlowContainer<T>)}.");

        _layoutChildren[gameObject] = newPosition;
        InvalidateLayout();
    }

    public void Insert(int position, T drawable)
    {
        Add(drawable);
        SetLayoutPosition(drawable, position);
    }

    public float GetLayoutPosition(GameObject gameObject)
    {
        if (!_layoutChildren.ContainsKey(gameObject))
            throw new InvalidOperationException($"Cannot get layout position of game object which is not contained within this {nameof(FlowContainer<T>)}.");

        return _layoutChildren[gameObject];
    }

    public virtual IEnumerable<GameObject> FlowingChildren => InternalChildren.Where(d => d.IsPresent).OrderBy(d => _layoutChildren[d]).ThenBy(d => d.ChildID);

    protected abstract IEnumerable<Vector2> ComputeLayoutPositions();

    private void performLayout()
    {
        OnLayout?.Invoke();

        if (!Children.Any())
            return;

        int processedCount = 0;

        using (IEnumerator<Vector2> positionEnumerator = ComputeLayoutPositions().GetEnumerator())
        using (IEnumerator<GameObject> gameObjectEnumerator = FlowingChildren.GetEnumerator())
        {
            while (true)
            {
                bool nextPos = positionEnumerator.MoveNext();
                bool nextGameObject = gameObjectEnumerator.MoveNext();

                if (nextPos != nextGameObject)
                {
                    throw new InvalidOperationException(
                        $"{GetType().FullName}.{nameof(ComputeLayoutPositions)} returned a total of {processedCount} positions for {FlowingChildren.Count()} children. {nameof(ComputeLayoutPositions)} must return 1 position per child.");
                }

                if (!nextPos)
                    return;

                var drawable = gameObjectEnumerator.Current;
                var pos = positionEnumerator.Current;

                processedCount++;

                Debug.Assert(drawable is not null);

                if (drawable.RelativePositionAxes != Axes.None)
                    throw new InvalidOperationException($"A flow container cannot contain a child with relative positioning (it is {drawable.RelativePositionAxes}).");

                Vector2 currentTargetPos = drawable.Position;

                if (currentTargetPos == pos) continue;

                drawable.Position = pos;
            }
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
            performLayout();
            _layout.Validate();
        }
    }
}
