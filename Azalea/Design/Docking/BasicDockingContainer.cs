using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using System;
using System.Numerics;

namespace Azalea.Design.Docking;
public class BasicDockingContainer : DockingContainer
{
	public Box NavigationBackground { get; }
	public Box ContentBackground { get; }
	public HollowBox ContentBorder { get; }
	public FlexContainer NavigationContainer { get; }

	private float _navigationHeight = 24;
	public float NavigationHeight
	{
		get => _navigationHeight;
		set
		{
			if (value == _navigationHeight) return;

			_navigationHeight = value;

			UpdateContentLayout();
			UpdateDockablesNavigation();
		}
	}

	private Boundary _contentPadding = new(6);
	public Boundary ContentPadding
	{
		get => _contentPadding;
		set
		{
			if (value == _contentPadding) return;

			_contentPadding = value;

			UpdateContentLayout();
		}
	}

	public BasicDockingContainer()
	{
		AddInternal(NavigationBackground = new Box()
		{
			Color = Palette.Black,
		});

		AddInternal(NavigationContainer = new FlexContainer());

		AddInternal(ContentBackground = new Box()
		{
			Color = Palette.Gray,
			Depth = 1000
		});

		AddInternal(ContentBorder = new HollowBox()
		{
			Thickness = 0,
			Color = Palette.Black,
			Depth = 999,
			OutsideContent = false
		});
	}

	private Boundary _lastBorderThickness = new(-1);
	protected override void Update()
	{
		base.Update();

		if (_lastBorderThickness != ContentBorder.Thickness)
		{
			UpdateContentLayout();
			_lastBorderThickness = ContentBorder.Thickness;
		}
	}

	protected override void UpdateDockablesNavigation()
	{
		NavigationContainer.Clear();

		foreach (var dockable in Dockables)
		{
			var navigationTab = CreateNavigationTab(dockable.Name, dockable == FocusedDockable);

			navigationTab.ClickAction = _ => FocusDockable(dockable);

			NavigationContainer.Add(navigationTab);
		}
	}

	protected virtual GameObject CreateNavigationTab(string name, bool focused)
		=> new BasicDockingContainerTab(name, focused, NavigationHeight);

	protected override void UpdateContentLayout()
	{
		NavigationBackground.Position = NavigationContainer.Position = Vector2.Zero;
		NavigationBackground.Size = NavigationContainer.Size = new Vector2(DrawWidth, _navigationHeight);

		ContentBackground.Position = ContentComposition.Position
			= ContentBorder.Position = new Vector2(0, _navigationHeight);
		ContentBackground.Size = ContentComposition.Size
			= ContentBorder.Size = DrawSize - new Vector2(0, _navigationHeight);

		ContentComposition.Position += new Vector2(_contentPadding.Left, _contentPadding.Right);
		ContentComposition.Position += new Vector2(ContentBorder.Thickness.Left, ContentBorder.Thickness.Top);
		ContentComposition.Size -= _contentPadding.Total + ContentBorder.Thickness.Total;
	}

	public override void Add(GameObject gameObject)
		=> throw new InvalidOperationException("Cannot add children directly to a DockingContainer");

	protected class BasicDockingContainerTab : Composition
	{
		public BasicDockingContainerTab(string title, bool focused, float heigth)
		{
			var titleText = new SpriteText()
			{
				Text = title,
				Anchor = Anchor.Center,
				Origin = Anchor.Center,
			};

			Size = new(titleText.Width + 20, heigth);
			BackgroundColor = focused ? Palette.Gray : Palette.Black;
			Add(titleText);
		}
	}
}
