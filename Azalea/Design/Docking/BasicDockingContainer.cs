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
	private const float __navigationHeight = 24;

	private Box _navigationBackground;
	private FlexContainer _navigationContainer;
	private Box _contentBackground;

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
		AddInternal(_navigationBackground = new Box()
		{
			Color = Palette.Black,
			Depth = 1000
		});

		AddInternal(_navigationContainer = new FlexContainer());

		AddInternal(_contentBackground = new Box()
		{
			Color = Palette.Gray,
			Depth = 1000
		});
	}

	protected override void UpdateDockablesNavigation()
	{
		_navigationContainer.Clear();

		foreach (var dockable in Dockables)
		{
			var title = new SpriteText()
			{
				Text = dockable.Name,
				Anchor = Anchor.Center,
				Origin = Anchor.Center,
			};

			_navigationContainer.Add(new Composition()
			{
				Size = new(title.Width + 20, __navigationHeight),
				BackgroundColor = dockable == FocusedDockable ? Palette.Gray : Palette.Black,
				Child = title,
				ClickAction = (e) => FocusDockable(dockable)
			});
		}
	}

	protected override void UpdateContentLayout()
	{
		_navigationBackground.Position = _navigationContainer.Position = Vector2.Zero;
		_navigationBackground.Size = _navigationContainer.Size = new Vector2(DrawWidth, __navigationHeight);

		_contentBackground.Position = ContentComposition.Position = new Vector2(0, __navigationHeight);
		_contentBackground.Size = ContentComposition.Size = DrawSize - new Vector2(0, __navigationHeight);

		ContentComposition.Position += new Vector2(_contentPadding.Left, _contentPadding.Right);
		ContentComposition.Size -= _contentPadding.Total;
	}

	public override void Add(GameObject gameObject)
		=> throw new InvalidOperationException("Cannot add children directly to a DockingContainer");
}
