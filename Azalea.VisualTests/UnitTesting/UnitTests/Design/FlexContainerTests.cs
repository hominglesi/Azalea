using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using System.Numerics;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Design;
public class FlexContainerTests : UnitTestSuite
{
	protected static readonly FlexContainer FlexContainer = new()
	{
		BackgroundColor = Palette.White
	};

	protected static void InitializeFlexContainer(Vector2 size,
		FlexDirection direction = FlexDirection.Horizontal,
		FlexWrapping wrapping = FlexWrapping.NoWrapping,
		Vector2? spacing = null,
		FlexContentAlignment contentAlignment = FlexContentAlignment.Start)
	{
		FlexContainer.Size = size;
		FlexContainer.Spacing = spacing ?? Vector2.Zero;
		FlexContainer.Direction = direction;
		FlexContainer.Wrapping = wrapping;
		FlexContainer.ContentAlignment = contentAlignment;
	}

	protected static void AddFlexItem(Vector2 size)
	{
		FlexContainer!.Add(new Box()
		{
			Size = size,
			Color = new Color(1, 0, 0, 0.1f * (FlexContainer.Children.Count + 1))
		});
	}

	protected static bool AssertPositions(Vector2[] positions)
	{
		if (positions.Length != FlexContainer.Children.Count)
			return false;

		FlexContainer.PerformLayout();

		for (int i = 0; i < positions.Length; i++)
		{
			if (FlexContainer.Children[i].Position != positions[i])
				return false;
		}

		return true;
	}

	public class DefaultLayoutTest : FlexContainerTest
	{
		public DefaultLayoutTest()
		{
			AddOperation("Create container", () => InitializeFlexContainer(new(500)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0),
				new(100, 0),
				new(200, 0)]));
		}
	}

	public class HorizontalReverseTest : FlexContainerTest
	{
		public HorizontalReverseTest()
		{
			AddOperation("Create horizontal reverse container",
				() => InitializeFlexContainer(new(500), FlexDirection.HorizontalReverse));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(400, 0),
				new(300, 0),
				new(200, 0)]));
		}
	}

	public class VerticalTest : FlexContainerTest
	{
		public VerticalTest()
		{
			AddOperation("Create vertical container",
				() => InitializeFlexContainer(new(500), FlexDirection.Vertical));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 000),
				new(0, 100),
				new(0, 200)]));
		}
	}

	public class VerticalReverseTest : FlexContainerTest
	{
		public VerticalReverseTest()
		{
			AddOperation("Create vertical reverse container",
				() => InitializeFlexContainer(new(500), FlexDirection.VerticalReverse));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 400),
				new(0, 300),
				new(0, 200)]));
		}
	}

	public class HorizontalWrapTest : FlexContainerTest
	{
		public HorizontalWrapTest()
		{
			AddOperation("Create horizontal wrap container",
				() => InitializeFlexContainer(new(250, 500), wrapping: FlexWrapping.Wrap));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 0),
				new(100, 0),
				new(0, 100),
				new(100, 100),
				new(0, 200)]));
		}
	}

	public class HorizontalReverseWrapTest : FlexContainerTest
	{
		public HorizontalReverseWrapTest()
		{
			AddOperation("Create horizontal reverse wrap container",
				() => InitializeFlexContainer(new(250, 500),
				FlexDirection.HorizontalReverse, FlexWrapping.Wrap));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(150, 0),
				new(50, 0),
				new(150, 100),
				new(50, 100),
				new(150, 200)]));
		}
	}

	public class VerticalWrapTest : FlexContainerTest
	{
		public VerticalWrapTest()
		{
			AddOperation("Create vertical wrap container",
				() => InitializeFlexContainer(new(500, 250),
				FlexDirection.Vertical, FlexWrapping.Wrap));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 0),
				new(0, 100),
				new(100, 0),
				new(100, 100),
				new(200, 0)]));
		}
	}

	public class VerticalReverseWrapTest : FlexContainerTest
	{
		public VerticalReverseWrapTest()
		{
			AddOperation("Create vertical reverse wrap container",
				() => InitializeFlexContainer(new(500, 250),
				FlexDirection.VerticalReverse, FlexWrapping.Wrap));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 150),
				new(0, 50),
				new(100, 150),
				new(100, 50),
				new(200, 150)]));
		}
	}

	public class WrapReverseTest : FlexContainerTest
	{
		public WrapReverseTest()
		{
			AddOperation("Create wrap reverse container",
				() => InitializeFlexContainer(new(250, 500), wrapping: FlexWrapping.WrapReverse));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 400),
				new(100, 400),
				new(0, 300),
				new(100, 300),
				new(0, 200)]));
		}
	}

	public class HorizontalReverseWrapReverseTest : FlexContainerTest
	{
		public HorizontalReverseWrapReverseTest()
		{
			AddOperation("Create horizontal reverse wrap reverse container",
				() => InitializeFlexContainer(new(250, 500),
				FlexDirection.HorizontalReverse, FlexWrapping.WrapReverse));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(150, 400),
				new(50, 400),
				new(150, 300),
				new(50, 300),
				new(150, 200)]));
		}
	}

	public class VerticalWrapReverseTest : FlexContainerTest
	{
		public VerticalWrapReverseTest()
		{
			AddOperation("Create vertical wrap reverse container",
				() => InitializeFlexContainer(new(500, 250),
				FlexDirection.Vertical, FlexWrapping.WrapReverse));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(400, 0),
				new(400, 100),
				new(300, 0),
				new(300, 100),
				new(200, 0)]));
		}
	}

	public class VerticalReverseWrapReverseTest : FlexContainerTest
	{
		public VerticalReverseWrapReverseTest()
		{
			AddOperation("Create vertical reverse wrap reverse container",
				() => InitializeFlexContainer(new(500, 250),
				FlexDirection.VerticalReverse, FlexWrapping.WrapReverse));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(400, 150),
				new(400, 50),
				new(300, 150),
				new(300, 50),
				new(200, 150)]));
		}
	}

	public class MainSpacingHorizontalTest : FlexContainerTest
	{
		public MainSpacingHorizontalTest()
		{
			AddOperation("Create horizontal container with spacing 10",
				() => InitializeFlexContainer(new(500, 500), spacing: new(10, 0)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 0),
				new(110, 0),
				new(220, 0)]));
		}
	}


	public class MainSpacingVerticalTest : FlexContainerTest
	{
		public MainSpacingVerticalTest()
		{
			AddOperation("Create vertical container with spacing 10",
				() => InitializeFlexContainer(new(500, 500),
				direction: FlexDirection.Vertical,
				spacing: new(0, 10)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 0),
				new(0, 110),
				new(0, 220)]));
		}
	}

	public class MainSpacingHorizontalReverseTest : FlexContainerTest
	{
		public MainSpacingHorizontalReverseTest()
		{
			AddOperation("Create horizontal reverse container with spacing 10",
				() => InitializeFlexContainer(new(500, 500),
				direction: FlexDirection.HorizontalReverse,
				spacing: new(10, 0)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(400, 0),
				new(290, 0),
				new(180, 0)]));
		}
	}

	public class SpacingOverflowsItemTest : FlexContainerTest
	{
		public SpacingOverflowsItemTest()
		{
			AddOperation("Create horizontal container with spacing 10",
				() => InitializeFlexContainer(new(300, 500),
				wrapping: FlexWrapping.Wrap,
				spacing: new(10, 0)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 0),
				new(110, 0),
				new(0, 100),
				new(110, 100)]));
		}
	}

	public class CrossSpacingHorizontalTest : FlexContainerTest
	{
		public CrossSpacingHorizontalTest()
		{
			AddOperation("Create horizontal container with spacing 10",
				() => InitializeFlexContainer(new(250, 500),
				wrapping: FlexWrapping.Wrap,
				spacing: new(0, 10)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 0),
				new(100, 0),
				new(0, 110),
				new(100, 110),
				new(0, 220)]));
		}
	}

	public class ContentAlignEndTest : FlexContainerTest
	{
		public ContentAlignEndTest()
		{
			AddOperation("Create wrapped content align end container",
				() => InitializeFlexContainer(new(250, 500),
				wrapping: FlexWrapping.Wrap,
				contentAlignment: FlexContentAlignment.End));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 200),
				new(100, 200),
				new(0, 300),
				new(100, 300),
				new(0, 400)]));
		}
	}

	public class ContentAlignCenterTest : FlexContainerTest
	{
		public ContentAlignCenterTest()
		{
			AddOperation("Create wrapped content align center container",
				() => InitializeFlexContainer(new(250, 500),
				wrapping: FlexWrapping.Wrap,
				contentAlignment: FlexContentAlignment.Center));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 100),
				new(100, 100),
				new(0, 200),
				new(100, 200),
				new(0, 300)]));
		}
	}

	public class ContentAlignSpaceBetweenTest : FlexContainerTest
	{
		public ContentAlignSpaceBetweenTest()
		{
			AddOperation("Create wrapped content align space between container",
				() => InitializeFlexContainer(new(250, 500),
				wrapping: FlexWrapping.Wrap,
				contentAlignment: FlexContentAlignment.SpaceBetween));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 0),
				new(100, 0),
				new(0, 200),
				new(100, 200),
				new(0, 400)]));
		}
	}

	public class ContentAlignSpaceEvenlyTest : FlexContainerTest
	{
		public ContentAlignSpaceEvenlyTest()
		{
			AddOperation("Create wrapped content align space evenly container",
				() => InitializeFlexContainer(new(250, 500),
				wrapping: FlexWrapping.Wrap,
				contentAlignment: FlexContentAlignment.SpaceEvenly));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 50),
				new(100, 50),
				new(0, 200),
				new(100, 200),
				new(0, 350)]));
		}
	}

	public class ContentAlignSpaceAroundTest : FlexContainerTest
	{
		public ContentAlignSpaceAroundTest()
		{
			AddOperation("Create wrapped content align space around container",
				() => InitializeFlexContainer(new(250, 600),
				wrapping: FlexWrapping.Wrap,
				contentAlignment: FlexContentAlignment.SpaceAround));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));
			AddOperation("Add item", () => AddFlexItem(new(100)));

			AddResult("Assert positions", () => AssertPositions(
				[new(0, 50),
				new(100, 50),
				new(0, 250),
				new(100, 250),
				new(0, 450)]));
		}
	}

	public abstract class FlexContainerTest : UnitTest
	{
		public override void Setup(UnitTestContainer scene)
		{
			base.Setup(scene);

			FlexContainer.Parent?.Remove(FlexContainer);
			scene.Add(FlexContainer);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			base.TearDown(scene);

			FlexContainer.Clear();
			scene.Remove(FlexContainer);
		}
	}
}
