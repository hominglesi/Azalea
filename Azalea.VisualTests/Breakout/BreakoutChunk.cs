using Azalea.Design.Shapes;
using Azalea.Numerics;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.VisualTests.Breakout;
public class BreakoutChunk
{
	public Rectangle BoundingBox { get; set; }
	public List<BreakoutChunk> SubChunks { get; set; } = new();
	public List<Box> Bricks { get; set; } = new();

	public BreakoutChunk(Rectangle boundingBox)
	{
		BoundingBox = boundingBox;
	}

	public void AddBrick(Box brick)
	{
		Bricks.Add(brick);
	}

	public bool RemoveBrick(Box brick)
	{
		foreach (var chunk in SubChunks)
		{
			if (chunk.Bricks.Contains(brick))
			{
				chunk.Bricks.Remove(brick);
				if (chunk.Bricks.Count == 0)
					SubChunks.Remove(chunk);

				return true;
			}

			if (chunk.RemoveBrick(brick))
			{
				if (chunk.SubChunks.Count == 0)
					SubChunks.Remove(chunk);

				return true;
			};
		}

		return false;
	}

	public void Subdivide(int times)
	{
		for (int i = 0; i < times; i++)
			Subdivide();
	}

	public void Subdivide()
	{
		if (SubChunks.Count != 0)
		{
			foreach (var chunk in SubChunks)
				chunk.Subdivide();
		}
		else
		{
			var childSize = BoundingBox.Size / 2;
			var topLeft = BoundingBox.TopLeft;
			var childBounds = new Rectangle(topLeft, childSize);

			SubChunks.Add(new BreakoutChunk(childBounds));
			childBounds.X += childSize.X;
			SubChunks.Add(new BreakoutChunk(childBounds));
			childBounds.Y += childSize.Y;
			SubChunks.Add(new BreakoutChunk(childBounds));
			childBounds.X -= childSize.X;
			SubChunks.Add(new BreakoutChunk(childBounds));

			foreach (var brick in Bricks)
			{
				foreach (var chunk in SubChunks)
				{
					if (chunk.BoundingBox.Contains(brick.Position))
					{
						chunk.AddBrick(brick);
						break;
					}
				}
			}
			Bricks.Clear();
		}
	}

	public void SubdivideHorizontaly()
	{
		if (SubChunks.Count != 0)
		{
			foreach (var chunk in SubChunks)
				chunk.SubdivideHorizontaly();
		}
		else
		{
			var childSize = BoundingBox.Size;
			childSize.X /= 2;
			var topLeft = BoundingBox.TopLeft;
			var childBounds = new Rectangle(topLeft, childSize);

			SubChunks.Add(new BreakoutChunk(childBounds));
			childBounds.X += childSize.X;
			SubChunks.Add(new BreakoutChunk(childBounds));

			foreach (var brick in Bricks)
			{
				foreach (var chunk in SubChunks)
				{
					if (chunk.BoundingBox.Contains(brick.Position))
					{
						chunk.AddBrick(brick);
						break;
					}
				}
			}
			Bricks.Clear();
		}
	}

	public BreakoutChunk? GetDeepestContaining(Vector2 point)
	{
		if (BoundingBox.Contains(point) == false)
			return null;

		if (SubChunks.Count == 0)
			return this;

		foreach (var chunk in SubChunks)
		{
			if (chunk.BoundingBox.Contains(point))
				return chunk.GetDeepestContaining(point);
		}

		return null;
	}

	public IEnumerable<BreakoutChunk> GetChunksIntersecting(Rectangle rect)
	{
		if (BoundingBox.Intersects(rect) == false)
			yield break;

		if (SubChunks.Count == 0)
			yield return this;

		foreach (var chunk in SubChunks)
		{
			foreach (var intersecting in chunk.GetChunksIntersecting(rect))
			{
				yield return intersecting;
			}
		}
	}
}
