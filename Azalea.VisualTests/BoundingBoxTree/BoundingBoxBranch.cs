using Azalea.Numerics;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.VisualTests.BoundingBoxTree;
public class BoundingBoxBranch
{
	public Rectangle BoundingBox { get; set; }

	private List<BoundingBoxBranch> Children { get; set; } = new();

	public BoundingBoxBranch(Rectangle boundingBox, int children)
	{
		BoundingBox = boundingBox;

		if (children > 0)
		{
			var childSize = boundingBox.Size / 2;
			var topLeft = BoundingBox.TopLeft;
			var childBounds = new Rectangle(topLeft, childSize);

			Children.Add(new BoundingBoxBranch(childBounds, children - 1));
			childBounds.X += childSize.X;
			Children.Add(new BoundingBoxBranch(childBounds, children - 1));
			childBounds.Y += childSize.Y;
			Children.Add(new BoundingBoxBranch(childBounds, children - 1));
			childBounds.X -= childSize.X;
			Children.Add(new BoundingBoxBranch(childBounds, children - 1));
		}
	}

	public ContainsLookup? ContainsPoint(Vector2 point)
	{
		var checkCount = 0;

		if (BoundingBox.Contains(point))
		{
			checkCount++;
			if (Children.Count == 0)
				return new ContainsLookup()
				{
					BoundingBox = BoundingBox,
					CheckCount = checkCount
				};

			foreach (var child in Children)
			{
				var childRect = child.ContainsPoint(point);
				if (childRect is null)
					checkCount++;
				else if (childRect is not null)
				{
					var returnedRect = childRect.Value;
					returnedRect.CheckCount += checkCount;
					return returnedRect;
				}
			}
		}

		return null;
	}

	public struct ContainsLookup
	{
		public Rectangle BoundingBox;
		public int CheckCount;
	}
}
