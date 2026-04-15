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

	private FlexContentAlignment _contentAlignment = FlexContentAlignment.Start;
	public FlexContentAlignment ContentAlignment
	{
		get => _contentAlignment;
		set
		{
			if (_contentAlignment == value) return;

			_contentAlignment = value;
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
		var layoutSize = ChildSize;

		var children = FlexChildren.ToArray();

		var positions = ArrayPool<Vector2>.Shared.Rent(children.Length);
		var lines = ArrayPool<Vector2WithInt>.Shared.Rent(children.Length);
		var nextLine = 0;

		try
		{
			var nextPosition = getMainStart();
			var nextItemLineIndex = 0;
			var mainLength = 0f;
			var totalMainLength = 0f;
			var maxMainLength = getMainLength(layoutSize);
			var crossLength = 0f;
			var totalCrossLength = 0f;
			var maxCrossLength = getCrossLength(layoutSize);

			// Layout pass
			for (int i = 0; i < children.Length; i++)
			{
				var itemMainLength = getMainLength(children[i]);

				if (Wrapping != FlexWrapping.NoWrapping && mainLength + itemMainLength > maxMainLength)
				{
					var crossSpacing = getCrossLength(Spacing);
					var crossOffset = crossLength + totalCrossLength + crossSpacing;
					nextPosition = getMainStart() + (crossOffset * getCrossDirection());

					totalMainLength += mainLength;
					totalCrossLength += crossLength + crossSpacing;
					lines[nextLine++] = new Vector2WithInt(mainLength, crossLength, nextItemLineIndex);
					mainLength = 0f;
					crossLength = 0f;
					nextItemLineIndex = 0;
				}

				var positionOffset = itemMainLength + getMainSpacing();

				positions[i] = nextPosition + getPositionOffset(children[i]);
				nextPosition += positionOffset * getMainDirection();
				crossLength = Math.Max(crossLength, getCrossLength(children[i]));
				mainLength += positionOffset;
				nextItemLineIndex++;
			}

			if (nextLine != lines.Length)
			{
				totalMainLength += mainLength;
				totalCrossLength += crossLength;
				lines[nextLine++] = new Vector2WithInt(mainLength, crossLength, nextItemLineIndex);
			}

			// Justification pass
			int index = 0;
			switch (Justification)
			{
				case FlexJustification.Center:
				case FlexJustification.End:
					var offsetRatio = Justification switch
					{
						FlexJustification.Center => 0.5f,
						_ => 1
					};

					for (int i = 0; i < nextLine; i++)
					{
						for (int j = 0; j < lines[i].Int; j++)
						{
							var mainOffset = (maxMainLength - lines[i].Position.X) * offsetRatio;
							positions[index++] += mainOffset * getMainDirection();
						}
					}
					break;
				case FlexJustification.SpaceBetween:
					for (int i = 0; i < nextLine; i++)
					{
						if (lines[i].Int <= 1)
							break;

						for (int j = 0; j < lines[i].Int; j++)
						{
							var mainOffset = maxMainLength - lines[i].Position.X;
							mainOffset /= lines[i].Int - 1;
							positions[index++] += (mainOffset * j) * getMainDirection();
						}
					}
					break;
				case FlexJustification.SpaceEvenly:
					for (int i = 0; i < nextLine; i++)
					{
						for (int j = 0; j < lines[i].Int; j++)
						{
							var mainOffset = maxMainLength - lines[i].Position.X;
							mainOffset /= (lines[i].Int - 1) + 2;
							positions[index++] += (mainOffset * (j + 1)) * getMainDirection();
						}
					}
					break;
				case FlexJustification.SpaceAround:
					for (int i = 0; i < nextLine; i++)
					{
						for (int j = 0; j < lines[i].Int; j++)
						{
							var mainOffset = maxMainLength - lines[i].Position.X;
							mainOffset /= lines[i].Int * 2;
							positions[index++] += (mainOffset * ((j * 2) + 1)) * getMainDirection();
						}
					}
					break;
			}

			// Item alignment pass
			index = 0;
			if (Alignment != FlexAlignment.Start)
			{
				var offsetRatio = _alignment switch
				{
					FlexAlignment.Center => 0.5f,
					_ => 1
				};

				for (int i = 0; i < nextLine; i++)
				{
					for (int j = 0; j < lines[i].Int; j++)
					{
						var crossOffset = (lines[i].Position.Y - getCrossLength(children[index])) * offsetRatio;
						positions[index++] += crossOffset * getCrossDirection();
					}
				}
			}

			// Content alignment pass
			var remainingCrossLength = maxCrossLength - totalCrossLength;

			Vector2 offset = Vector2.Zero;
			index = 0;
			switch (ContentAlignment)
			{
				case FlexContentAlignment.End:
					offset = remainingCrossLength * getCrossDirection();
					for (int i = 0; i < positions.Length; i++)
						positions[i] += offset;
					break;
				case FlexContentAlignment.Center:
					offset = (remainingCrossLength / 2) * getCrossDirection();
					for (int i = 0; i < positions.Length; i++)
						positions[i] += offset;
					break;
				case FlexContentAlignment.SpaceBetween:
					if (nextLine <= 1)
						break;

					offset = remainingCrossLength * getCrossDirection();
					offset /= nextLine - 1;

					for (int i = 0; i <= nextLine; i++)
						for (int j = 0; j < lines[i].Int; j++)
							positions[index++] += offset * i;
					break;
				case FlexContentAlignment.SpaceEvenly:
					offset = remainingCrossLength * getCrossDirection();
					offset /= (nextLine - 1) + 2;

					for (int i = 0; i <= nextLine; i++)
						for (int j = 0; j < lines[i].Int; j++)
							positions[index++] += offset * (i + 1);
					break;
				case FlexContentAlignment.SpaceAround:
					offset = remainingCrossLength * getCrossDirection();
					offset /= nextLine * 2;

					for (int i = 0; i <= nextLine; i++)
						for (int j = 0; j < lines[i].Int; j++)
							positions[index++] += offset * ((i * 2) + 1);

					break;

			}

			for (int i = 0; i < children.Length; i++)
				yield return positions[i];
		}
		finally
		{
			ArrayPool<Vector2>.Shared.Return(positions);
			ArrayPool<Vector2WithInt>.Shared.Return(lines);
		}
	}

	struct Vector2WithInt(float x, float y, int @int)
	{
		public Vector2 Position = new(x, y);
		public int Int = @int;
	}

	private Vector2 getMainDirection()
		=> Direction switch
		{
			FlexDirection.Horizontal => new(1, 0),
			FlexDirection.HorizontalReverse => new(-1, 0),
			FlexDirection.Vertical => new(0, 1),
			_ => new(0, -1),
		};

	private Vector2 getCrossDirection()
	{
		var axisDir = Wrapping == FlexWrapping.WrapReverse ? -1 : 1;

		return Direction switch
		{
			FlexDirection.Horizontal or FlexDirection.HorizontalReverse => new(0, axisDir),
			_ => new(axisDir, 0),
		};
	}

	private Vector2 getMainStart()
	{
		var crossStart = 0f;
		if (Wrapping == FlexWrapping.WrapReverse)
		{
			crossStart = Direction switch
			{
				FlexDirection.Horizontal or FlexDirection.HorizontalReverse => ChildSize.Y,
				_ => ChildSize.X
			};
		}

		return Direction switch
		{
			FlexDirection.Horizontal => new(0, crossStart),
			FlexDirection.HorizontalReverse => new(ChildSize.X, crossStart),
			FlexDirection.Vertical => new(crossStart, 0),
			_ => new(crossStart, ChildSize.Y),
		};
	}

	private float getMainLength(GameObject obj) => getMainLength(obj.BoundingBox.Size);

	private float getMainLength(Vector2 vector)
		=> Direction switch
		{
			FlexDirection.Horizontal or FlexDirection.HorizontalReverse => vector.X,
			_ => vector.Y
		};

	private float getCrossLength(GameObject obj) => getCrossLength(obj.BoundingBox.Size);
	private float getCrossLength(Vector2 vector)
		=> Direction switch
		{
			FlexDirection.Horizontal or FlexDirection.HorizontalReverse => vector.Y,
			_ => vector.X
		};

	private float getMainSpacing()
		=> Direction switch
		{
			FlexDirection.Horizontal or FlexDirection.HorizontalReverse => Spacing.X,
			_ => Spacing.Y
		};

	private Vector2 getPositionOffset(GameObject obj)
	{
		var offset = Vector2.Zero;

		if (Direction == FlexDirection.HorizontalReverse)
			offset.X -= obj.Width;
		else if (Direction == FlexDirection.VerticalReverse)
			offset.Y -= obj.Height;

		if (Wrapping == FlexWrapping.WrapReverse)
		{
			if (Direction == FlexDirection.Horizontal || Direction == FlexDirection.HorizontalReverse)
				offset.Y -= obj.Height;
			else if (Direction == FlexDirection.Vertical || Direction == FlexDirection.VerticalReverse)
				offset.X -= obj.Width;
		}

		return offset;
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
	WrapReverse
}

public enum FlexJustification
{
	Start,
	End,
	Center,
	SpaceBetween,
	SpaceAround,
	SpaceEvenly
}

public enum FlexAlignment
{
	Start,
	End,
	Center
}

public enum FlexContentAlignment
{
	Start,
	End,
	Center,
	SpaceBetween,
	SpaceAround,
	SpaceEvenly
}
