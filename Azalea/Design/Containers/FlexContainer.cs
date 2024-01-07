using Azalea.Graphics;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Azalea.Design.Containers;

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

	private FlexWrapping _wrapping = FlexWrapping.NoWrapping;
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

	private FlexJustification _justification = FlexJustification.Start;
	public FlexJustification Justification
	{
		get => _justification;
		set
		{
			if (_justification == value) return;

			_justification = value;
			InvalidateLayout();
		}
	}

	private FlexAlignment _alignment = FlexAlignment.Start;
	public FlexAlignment Alignment
	{
		get => _alignment;
		set
		{
			if (_alignment == value) return;

			_alignment = value;
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

	private void orderByDirectionHorizontally(ref Vector2[] positions, ref GameObject[] children)
	{
		var offset = 0f;
		for (int i = 0; i < children.Length; i++)
		{
			positions[i].X = offset;
			offset += children[i].Width;
			offset += Spacing.X;
		}
	}

	private void orderByDirectionVertically(ref Vector2[] positions, ref GameObject[] children)
	{
		var offset = 0f;
		for (int i = 0; i < children.Length; i++)
		{
			positions[i].Y = offset;
			offset += children[i].Height;
			offset += Spacing.Y;
		}
	}

	private int getNextChunkHorizontally(ref Vector2[] positions, ref GameObject[] children, int startIndex, float maxLength)
	{
		var length = 0f;
		for (int i = startIndex; i < children.Length; i++)
		{
			length += children[i].Width;
			if (length > maxLength || children[i] is FlowNewLine)
			{
				if (i == startIndex) return startIndex;
				return i - 1;
			}
			length += Spacing.X;
		}

		return children.Length - 1;
	}

	private int getNextChunkVertically(ref Vector2[] positions, ref GameObject[] children, int startIndex, float maxLength)
	{
		var length = 0f;
		for (int i = startIndex; i < children.Length; i++)
		{
			length += children[i].Width;
			if (length > maxLength || children[i] is FlowNewLine)
			{
				if (i == startIndex) return startIndex;
				return i - 1;
			}
			length += Spacing.X;
		}

		return children.Length - 1;
	}

	private float getMaxHeight(ref GameObject[] children, int startIndex, int endIndex)
	{
		var max = 0f;
		for (int i = startIndex; i <= endIndex; i++)
		{
			max = MathF.Max(max, children[i].Height);
		}
		return max;
	}

	private float getMaxWidth(ref GameObject[] children, int startIndex, int endIndex)
	{
		var max = 0f;
		for (int i = startIndex; i <= endIndex; i++)
		{
			max = MathF.Max(max, children[i].Width);
		}
		return max;
	}

	private float getTotalWidth(ref GameObject[] children, int startIndex, int endIndex)
	{
		var total = 0f;
		for (int i = startIndex; i <= endIndex; i++)
		{
			total += children[i].Width;
			total += _spacing.X;
		}
		total -= _spacing.X;

		return total;
	}

	private float getTotalHeight(ref GameObject[] children, int startIndex, int endIndex)
	{
		var total = 0f;
		for (int i = startIndex; i <= endIndex; i++)
		{
			total += children[i].Height;
			total += _spacing.Y;
		}
		total -= _spacing.Y;

		return total;
	}

	private void placeRow(ref Vector2[] positions, int startIndex, int endIndex, float x, float y)
	{
		var firstX = positions[startIndex].X;
		for (int i = startIndex; i <= endIndex; i++)
		{
			positions[i].X += x;
			positions[i].X -= firstX;
			positions[i].Y = y;
		}
	}

	private void placeRowReverse(ref Vector2[] positions, ref GameObject[] children, int startIndex, int endIndex, float width, float x, float y)
	{
		var firstX = positions[startIndex].X;
		for (int i = startIndex; i <= endIndex; i++)
		{
			var xOffset = positions[i].X - firstX;
			positions[i].X = width - children[i].Width - xOffset - x;
			positions[i].Y = y;
		}
	}

	private void placeColumn(ref Vector2[] positions, int startIndex, int endIndex, float x, float y)
	{
		var firstY = positions[startIndex].Y;
		for (int i = startIndex; i <= endIndex; i++)
		{
			positions[i].Y += y;
			positions[i].Y -= firstY;
			positions[i].X = x;
		}
	}

	private void placeColumnReverse(ref Vector2[] positions, ref GameObject[] children, int startIndex, int endIndex, float height, float x, float y)
	{
		var firstY = positions[startIndex].Y;
		for (int i = startIndex; i <= endIndex; i++)
		{
			var yOffset = positions[i].Y - firstY;
			positions[i].Y = height - children[i].Height - yOffset - y;
			positions[i].X = x;
		}
	}

	private void alignRow(ref Vector2[] positions, ref GameObject[] children, int startIndex, int endIndex)
	{
		if (Alignment == FlexAlignment.Start) return;

		var maxHeight = getMaxHeight(ref children, startIndex, endIndex);

		for (int i = startIndex; i <= endIndex; i++)
		{
			var offset = maxHeight - children[i].Height;
			if (Alignment == FlexAlignment.Center)
				offset /= 2;
			positions[i].Y += offset;
		}

	}

	private void alignColumn(ref Vector2[] positions, ref GameObject[] children, int startIndex, int endIndex)
	{
		if (Alignment == FlexAlignment.Start) return;

		var maxWidth = getMaxWidth(ref children, startIndex, endIndex);

		for (int i = startIndex; i <= endIndex; i++)
		{
			var offset = maxWidth - children[i].Width;
			if (Alignment == FlexAlignment.Center)
				offset /= 2;
			positions[i].X += offset;
		}

	}

	protected override IEnumerable<Vector2> ComputeLayoutPositions()
	{
		var layoutSize = ChildSize;

		var children = FlexChildren.ToArray();
		var positions = ArrayPool<Vector2>.Shared.Rent(children.Length);

		try
		{

			if (Direction == FlexDirection.Horizontal || Direction == FlexDirection.HorizontalReverse)
			{
				orderByDirectionHorizontally(ref positions, ref children);

				int startIndex = 0;
				var verticalOffset = 0f;

				while (true)
				{
					var endIndex = getNextChunkHorizontally(ref positions, ref children, startIndex, layoutSize.X);
					var contentWidth = getTotalWidth(ref children, startIndex, endIndex);
					var xOffset = Justification switch
					{
						FlexJustification.End => layoutSize.X - contentWidth,
						FlexJustification.Center => (layoutSize.X - contentWidth) / 2,
						_ => 0
					};
					if (Direction == FlexDirection.Horizontal)
						placeRow(ref positions, startIndex, endIndex, xOffset, verticalOffset);
					else
						placeRowReverse(ref positions, ref children, startIndex, endIndex, layoutSize.X, xOffset, verticalOffset);

					alignRow(ref positions, ref children, startIndex, endIndex);

					verticalOffset += getMaxHeight(ref children, startIndex, endIndex);
					verticalOffset += Spacing.Y;

					if (children[endIndex] is FlowNewLine nl)
						verticalOffset += nl.Length;

					if (endIndex >= children.Length - 1)
						break;

					startIndex = endIndex + 1;
				}


			}
			else
			{
				orderByDirectionVertically(ref positions, ref children);

				int startIndex = 0;
				var horizontalOffset = 0f;

				while (true)
				{
					var endIndex = getNextChunkVertically(ref positions, ref children, startIndex, layoutSize.Y);
					var contentHeight = getTotalHeight(ref children, startIndex, endIndex);
					var yOffset = Justification switch
					{
						FlexJustification.End => layoutSize.Y - contentHeight,
						FlexJustification.Center => (layoutSize.Y - contentHeight) / 2,
						_ => 0
					};
					if (Direction == FlexDirection.Vertical)
						placeColumn(ref positions, startIndex, endIndex, horizontalOffset, yOffset);
					else
						placeColumnReverse(ref positions, ref children, startIndex, endIndex, layoutSize.Y, horizontalOffset, yOffset);

					alignColumn(ref positions, ref children, startIndex, endIndex);

					horizontalOffset += getMaxWidth(ref children, startIndex, endIndex);
					horizontalOffset += Spacing.X;

					if (children[endIndex] is FlowNewLine nl)
						horizontalOffset += nl.Length;

					if (endIndex >= children.Length - 1)
						break;

					startIndex = endIndex + 1;
				}
			}



			for (int i = 0; i < children.Length; i++)
			{
				yield return positions[i];
			}
		}
		finally
		{
			ArrayPool<Vector2>.Shared.Return(positions);
		}
	}
}
/// <summary>
/// Represents the direction children of a <see cref="FlexComposition{T}"/> should be filled in.
/// </summary>
public enum FlexDirection
{
	Horizontal,
	HorizontalReverse,
	Vertical,
	VerticalReverse
}

/// <summary>
/// Represents if and how the children of a <see cref="FlexComposition{T}"/> should be filled in.
/// </summary>
public enum FlexWrapping
{
	NoWrapping,
	Wrap,
}

public enum FlexJustification
{
	Start,
	End,
	Center
}

public enum FlexAlignment
{
	Start,
	End,
	Center
}
