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

	private void orderByDirection(ref Vector2[] positions, ref GameObject[] children)
	{
		var offset = 0f;
		for (int i = 0; i < children.Length; i++)
		{
			positions[i] = createMainVector(offset);
			offset += getMainLength(children[i]);
			offset += getMainSpacing();
		}
	}

	private int getNextChunk(ref Vector2[] positions, ref GameObject[] children, int startIndex, float maxLength)
	{
		var length = 0f;
		for (int i = startIndex; i < children.Length; i++)
		{
			length += getMainLength(children[i]);
			if (length > maxLength || children[i] is FlowNewLine)
			{
				if (i == startIndex) return startIndex;
				return i - 1;
			}
			length += getMainSpacing();
		}

		return children.Length - 1;
	}

	private float getMaxSideLength(ref GameObject[] children, int startIndex, int endIndex)
	{
		var max = 0f;
		for (int i = startIndex; i <= endIndex; i++)
			max = MathF.Max(max, getSideLength(children[i]));

		return max;
	}

	private float getTotalLength(ref GameObject[] children, int startIndex, int endIndex)
	{
		var total = 0f;
		for (int i = startIndex; i <= endIndex; i++)
		{
			total += getMainLength(children[i]);
			total += getMainSpacing();
		}
		total -= getMainSpacing();

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
			positions[i].X = width - children[i].LayoutRectangle.Width - xOffset - x;
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
			positions[i].Y = height - children[i].LayoutRectangle.Height - yOffset - y;
			positions[i].X = x;
		}
	}

	private void align(ref Vector2[] positions, ref GameObject[] children, int startIndex, int endIndex)
	{
		if (Alignment == FlexAlignment.Start) return;

		var maxLength = getMaxSideLength(ref children, startIndex, endIndex);

		for (int i = startIndex; i <= endIndex; i++)
		{
			var offset = maxLength - getSideLength(children[i]);
			if (Alignment == FlexAlignment.Center)
				offset /= 2;
			positions[i] += createSideVector(offset);
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
				orderByDirection(ref positions, ref children);

				if (Wrapping == FlexWrapping.NoWrapping)
				{
					var startIndex = 0;
					var endIndex = children.Length - 1;

					if (Direction == FlexDirection.Horizontal)
						placeRow(ref positions, startIndex, endIndex, 0, 0);
					else
					{
						var totalWidth = getTotalLength(ref children, startIndex, endIndex);
						placeRowReverse(ref positions, ref children, startIndex, endIndex, totalWidth, 0, 0);
					}

					align(ref positions, ref children, startIndex, endIndex);
				}
				else
				{
					int startIndex = 0;
					var verticalOffset = 0f;

					while (true)
					{
						var endIndex = getNextChunk(ref positions, ref children, startIndex, layoutSize.X);
						var contentWidth = getTotalLength(ref children, startIndex, endIndex);
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

						align(ref positions, ref children, startIndex, endIndex);

						verticalOffset += getMaxSideLength(ref children, startIndex, endIndex);
						verticalOffset += Spacing.Y;

						if (children[endIndex] is FlowNewLine nl)
							verticalOffset += nl.Length;

						if (endIndex >= children.Length - 1)
							break;

						startIndex = endIndex + 1;
					}
				}
			}
			else
			{
				orderByDirection(ref positions, ref children);

				if (Wrapping == FlexWrapping.NoWrapping)
				{
					var startIndex = 0;
					var endIndex = children.Length - 1;

					if (Direction == FlexDirection.Vertical)
						placeColumn(ref positions, startIndex, endIndex, 0, 0);
					else
					{
						var totalHeight = getTotalLength(ref children, startIndex, endIndex);
						placeColumnReverse(ref positions, ref children, startIndex, endIndex, totalHeight, 0, 0);
					}

					align(ref positions, ref children, startIndex, endIndex);
				}
				else
				{
					int startIndex = 0;
					var horizontalOffset = 0f;

					while (true)
					{
						var endIndex = getNextChunk(ref positions, ref children, startIndex, layoutSize.Y);
						var contentHeight = getTotalLength(ref children, startIndex, endIndex);
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

						align(ref positions, ref children, startIndex, endIndex);

						horizontalOffset += getMaxSideLength(ref children, startIndex, endIndex);
						horizontalOffset += Spacing.X;

						if (children[endIndex] is FlowNewLine nl)
							horizontalOffset += nl.Length;

						if (endIndex >= children.Length - 1)
							break;

						startIndex = endIndex + 1;
					}
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

	private float getMainLength(GameObject obj)
		=> isHorizontal() ? obj.BoundingBox.Width : obj.BoundingBox.Height;

	private float getMainSpacing()
		=> isHorizontal() ? Spacing.X : Spacing.Y;

	private Vector2 createMainVector(float value)
		=> isHorizontal() ? new(value, 0) : new(0, value);

	private Vector2 createSideVector(float value)
		=> isHorizontal() ? new(0, value) : new(value, 0);

	private float getSideLength(GameObject obj)
		=> isHorizontal() ? obj.BoundingBox.Height : obj.BoundingBox.Width;

	private bool isHorizontal() => Direction == FlexDirection.Horizontal || Direction == FlexDirection.HorizontalReverse;
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
