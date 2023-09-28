using Azalea.Utils;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Azalea.Graphics.Containers;

public class FlexContainer : FlowContainer
{
	private FlexDirection _direction = FlexDirection.Horizontal;
	public FlexDirection Direction
	{
		get => _direction;
		set
		{
			if (_direction == value) return;
			_direction = value;
			InvalidateLayout();
		}
	}

	private FlexWrapping _wrapping = FlexWrapping.Wrap;
	public FlexWrapping Wrapping
	{
		get => _wrapping;
		set
		{
			if (_wrapping == value) return;
			_wrapping = value;
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

	protected override IEnumerable<Vector2> ComputeLayoutPositions()
	{
		var maxSize = ChildSize;

		var children = FlowingChildren.ToArray();
		if (children.Length <= 0)
			yield break;

		var layoutPositions = ArrayPool<Vector2>.Shared.Rent(children.Length);
		float rowHeight = 0;
		float rowWidth = 0;
		Vector2 current = Vector2.Zero;
		float rowStart = 0;

		switch (Direction)
		{
			case FlexDirection.HorizontalReverse:
				current = new Vector2(maxSize.X, 0);
				rowStart = maxSize.X;
				break;
			case FlexDirection.VerticalReverse:
				current = new Vector2(0, maxSize.Y);
				rowStart = maxSize.Y;
				break;
		}

		try
		{
			for (int i = 0; i < children.Length; i++)
			{
				GameObject c = children[i];

				Vector2 stride = c.BoundingBox.Size;
				stride += Spacing;

				if (Wrapping == FlexWrapping.Wrap)
				{
					if ((Direction == FlexDirection.Horizontal || Direction == FlexDirection.HorizontalReverse)
						&& Precision.DefinitelyBigger(rowWidth + stride.X, maxSize.X))
					{
						current.X = rowStart;
						current.Y += rowHeight;

						rowWidth = 0;
						rowHeight = 0;
					}
					else if ((Direction == FlexDirection.Vertical || Direction == FlexDirection.VerticalReverse)
						&& Precision.DefinitelyBigger(rowWidth + stride.Y, maxSize.Y))
					{
						current.X += rowHeight;
						current.Y = rowStart;

						rowWidth = 0;
						rowHeight = 0;
					}
				}

				switch (Direction)
				{
					case FlexDirection.Horizontal:
						layoutPositions[i] = current;
						if (stride.Y > rowHeight)
							rowHeight = stride.Y;
						current.X += stride.X;
						rowWidth += stride.X;
						break;
					case FlexDirection.HorizontalReverse:
						if (stride.Y > rowHeight)
							rowHeight = stride.Y;
						current.X -= stride.X;
						rowWidth += stride.X;
						layoutPositions[i] = current;
						break;
					case FlexDirection.Vertical:
						layoutPositions[i] = current;
						if (stride.X > rowHeight)
							rowHeight = stride.X;
						current.Y += stride.Y;
						rowWidth += stride.Y;
						break;
					case FlexDirection.VerticalReverse:
						if (stride.X > rowHeight)
							rowHeight = stride.X;
						current.Y -= stride.Y;
						rowWidth += stride.Y;
						layoutPositions[i] = current;
						break;
				}

				yield return layoutPositions[i];
			}
		}
		finally
		{
			ArrayPool<Vector2>.Shared.Return(layoutPositions);
		}
	}
}
/// <summary>
/// Represents the direction children of a <see cref="FlexContainer{T}"/> should be filled in.
/// </summary>
public enum FlexDirection
{
	Horizontal,
	HorizontalReverse,
	Vertical,
	VerticalReverse
}

/// <summary>
/// Represents if and how the children of a <see cref="FlexContainer{T}"/> should be filled in.
/// </summary>
public enum FlexWrapping
{
	NoWrapping,
	Wrap,
}
