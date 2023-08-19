using Azalea.Extentions.EnumExtentions;
using Azalea.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Azalea.Graphics.Containers;

/// <summary>
/// A container that fills space by arranging its children next to each other
/// The flow direction can be specified using <see cref="Direction"/>
/// 
/// </summary>
public class FillFlowContainer : FillFlowContainer<GameObject>
{

}

public class FillFlowContainer<T> : FlowContainer<T>
    where T : GameObject
{
    private FillDirection _direction = FillDirection.Full;

    public FillDirection Direction
    {
        get => _direction;
        set
        {
            if (_direction == value) return;

            _direction = value;
            InvalidateLayout();
        }
    }

    private FlowDirection _flowDirection = FlowDirection.Default;

    public FlowDirection FlowDirection
    {
        get => _flowDirection;
        set
        {
            if (_flowDirection == value) return;

            _flowDirection = value;
            InvalidateLayout();
        }
    }

    private Vector2 _spacing;

    public Vector2 Spacing
    {
        get => _spacing;
        set
        {
            if (_spacing == value) return;

            _spacing = value;
            InvalidateLayout();
        }
    }

    private Vector2 spacingFactor(GameObject c)
    {
        Vector2 result = c.RelativeOriginPosition;
        if (c.Anchor.HasFlagFast(Anchor.x2))
            result.X = 1 - result.X;
        if (c.Anchor.HasFlagFast(Anchor.y2))
            result.Y = 1 - result.Y;
        return result;
    }

    protected override IEnumerable<Vector2> ComputeLayoutPositions()
    {
        var max = MaximumSize;

        if (max == Vector2.Zero)
        {
            var s = ChildSize;

            max.X = AutoSizeAxes.HasFlagFast(Axes.X) ? float.MaxValue : s.X;
            max.Y = AutoSizeAxes.HasFlagFast(Axes.Y) ? float.MinValue : s.Y;
        }

        var children = FlowingChildren.ToArray();
        if (children.Length == 0)
            yield break;

        var layoutPositions = ArrayPool<Vector2>.Shared.Rent(children.Length);

        int[] rowIndices = ArrayPool<int>.Shared.Rent(children.Length);

        var rowOffsetsToMiddle = new List<float> { 0 };

        float rowHeight = 0;
        float rowBeginOffset = 0;
        var current = Vector2.Zero;

        Vector2 size = Vector2.Zero;

        try
        {
            for (int i = 0; i < children.Length; i++)
            {
                GameObject c = children[i];

                static Axes toAxes(FillDirection direction)
                {
                    return direction switch
                    {
                        FillDirection.Full => Axes.Both,
                        FillDirection.Horizontal => Axes.X,
                        FillDirection.Vertical => Axes.Y,
                        _ => throw new ArgumentException($"{direction} is not defined")
                    };
                }

                if ((c.RelativeSizeAxes & AutoSizeAxes & toAxes(Direction)) != 0
                    && (c.FillMode != FillMode.Fit || c.RelativeSizeAxes != Axes.Both || c.Size.X > RelativeChildSize.X
                    || c.Size.Y > RelativeChildSize.Y || AutoSizeAxes == Axes.Both))
                    throw new InvalidOperationException(
                        "Game objects inside a fill flow container may not have a relative size axis that the fill flow container is filling in and auto sizing for. " +
                        $"The fill flow container is set to flow in the {Direction} direction and autosize in {AutoSizeAxes} axes and the child is set to relative size in {c.RelativeSizeAxes} axes.");

                if (i == 0)
                {
                    size = c.BoundingBox.Size;
                    rowBeginOffset = spacingFactor(c).X * size.X;
                }

                float rowWidth = rowBeginOffset + current.X + (1 - spacingFactor(c).X) * size.X;

                if (Direction != FillDirection.Horizontal && (Precision.DefinitelyBigger(rowWidth, max.X)
                    || Direction == FillDirection.Vertical || ForceNewRow(c)))
                {
                    current.X = 0;
                    current.Y += rowHeight;

                    layoutPositions[i] = current;

                    rowOffsetsToMiddle.Add(0);
                    rowBeginOffset = spacingFactor(c).X * size.X;

                    rowHeight = 0;
                }
                else
                {
                    layoutPositions[i] = current;

                    rowOffsetsToMiddle[^1] = rowBeginOffset - rowWidth / 2;
                }

                rowIndices[i] = rowOffsetsToMiddle.Count - 1;
                Vector2 stride = Vector2.Zero;

                if (i < children.Length - 1)
                {
                    stride = (Vector2.One - spacingFactor(c)) * size;

                    c = children[i + 1];
                    size = c.BoundingBox.Size;

                    stride += spacingFactor(c) * size;
                }

                stride += Spacing;

                if (stride.Y > rowHeight)
                    rowHeight = stride.Y;
                current.X += stride.X;
            }

            float height = layoutPositions[children.Length - 1].Y;

            Vector2 ourRelativeAnchor = children[0].RelativeAnchorPosition;

            for (int i = 0; i < children.Length; i++)
            {
                var c = children[i];

                switch (Direction)
                {
                    case FillDirection.Vertical:
                        if (c.RelativeAnchorPosition.Y != ourRelativeAnchor.Y)
                        {
                            throw new InvalidOperationException(
                                    $"All game objects in a {nameof(FillFlowContainer)} must use the same RelativeAnchorPosition for the given {nameof(FillDirection)}({Direction}) ({ourRelativeAnchor.Y} != {c.RelativeAnchorPosition.Y}). "
                                    + $"Consider using multiple instances of {nameof(FillFlowContainer)} if this is intentional.");
                        }

                        break;
                    case FillDirection.Horizontal:
                        if (c.RelativeAnchorPosition.X != ourRelativeAnchor.X)
                        {
                            throw new InvalidOperationException(
                                    $"All game objects in a {nameof(FillFlowContainer)} must use the same RelativeAnchorPosition for the given {nameof(FillDirection)}({Direction}) ({ourRelativeAnchor.X} != {c.RelativeAnchorPosition.X}). "
                                    + $"Consider using multiple instances of {nameof(FillFlowContainer)} if this is intentional.");
                        }

                        break;
                    default:
                        if (c.RelativeAnchorPosition != ourRelativeAnchor)
                        {
                            throw new InvalidOperationException(
                                $"All game objects in a {nameof(FillFlowContainer)} must use the same RelativeAnchorPosition for the given {nameof(FillDirection)}({Direction}) ({ourRelativeAnchor} != {c.RelativeAnchorPosition}). "
                                + $"Consider using multiple instances of {nameof(FillFlowContainer)} if this is intentional.");
                        }

                        break;
                }

                var layoutPosition = layoutPositions[i];
                if (c.Anchor.HasFlagFast(Anchor.x1))
                    layoutPosition.X += rowOffsetsToMiddle[rowIndices[i]];
                else if (c.Anchor.HasFlagFast(Anchor.x2))
                    layoutPosition.X = -layoutPosition.X;

                if (c.Anchor.HasFlagFast(Anchor.y1))
                    layoutPosition.Y -= height / 2;
                else if (c.Anchor.HasFlagFast(Anchor.y2))
                    layoutPosition.Y = -layoutPosition.Y;

                yield return layoutPosition;
            }
        }
        finally
        {
            ArrayPool<Vector2>.Shared.Return(layoutPositions);
            ArrayPool<int>.Shared.Return(rowIndices);
        }
    }

    protected virtual bool ForceNewRow(GameObject child) => false;
}

/// <summary>
/// Represents the direction children of a <see cref="FillFlowContainer{T}"/> should be filled in.
/// </summary>
public enum FillDirection
{
    /// <summary>
    /// Fill horizontally first, then fill vertically
    /// </summary>
    Full,
    /// <summary>
    /// Fill only horizontaly
    /// </summary>
    Horizontal,
    /// <summary>
    /// Fill only vertically
    /// </summary>
    Vertical
}

/// <summary>
/// Reperesents if the children should be filled in Left-To-Right and Top-To-Bottom
/// </summary>
[Flags]
public enum FlowDirection
{
    /// <summary>
    /// Left to right and Top to bottom
    /// </summary>
    Default = 0,

    //Covering all usage cases
    LeftToRight = 0,
    TopToBottom = 0,

    RightToLeft = 1,
    BottomToTop = 1 << 1
}