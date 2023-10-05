using Azalea.Design.Containers;
using Azalea.Extentions.EnumExtentions;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.UserInterface;
using Azalea.Layout;

namespace Azalea.Debugging;
public class DebugSceneGraph : FlexContainer
{
	private Checkbox _checkbox;
	private FlexContainer _content;

	private GameObject _rootObject;
	private int _childCount;

	public DebugSceneGraph(GameObject rootObject)
	{
		_rootObject = rootObject;
		Wrapping = FlexWrapping.NoWrapping;
		AutoSizeAxes = Axes.Y;

		//if (_rootObject is Composition)
		//{
		Add(_checkbox = new Checkbox()
		{
			Size = new(25, 25)
		});
		_checkbox.Toggled += toggleChildren;
		//}

		Add(_content = new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
			Wrapping = FlexWrapping.NoWrapping,
			AutoSizeAxes = Axes.Y
		});

		_content.OnLayout += () =>
		{
			CompositeGameObject? parent = _content;
			while (parent != null)
			{
				parent.Invalidate(Invalidation.RequiredParentSizeToFit, InvalidationSource.Child);

				parent = parent.Parent;
			}
		};

		if (rootObject is Composition comp)
		{
			rootObject.Invalidated += (_, invalidation) =>
			{
				if (invalidation.HasFlagFast(Invalidation.Presence))
				{
					if (_childCount != comp.Children.Count)
						toggleChildren(_checkbox.Checked);
				}
			};
		}

		toggleChildren(false);

		//if (_rootObject is not Composition)
		//{
		//_content.Margin = new(0, 0, 0, 25);
		//}
	}

	private void toggleChildren(bool shown)
	{
		_content.Clear();
		_content.Add(createLabel(_rootObject));

		if (shown)
		{
			if (_rootObject is Composition comp)
			{
				foreach (var child in comp.Children)
				{
					var childGraph = new DebugSceneGraph(child);
					_content.Add(childGraph);
				}
			}
		}

		if (_rootObject is Composition composition)
		{
			_childCount = composition.Children.Count;
		}
	}

	private SpriteText createLabel(GameObject rootObject)
	{
		var label = new SpriteText()
		{
			Text = _rootObject.GetType().Name,
		};
		label.Click += (_) =>
		{
			AzaleaGame.Main.Host.Root.AddInternal(new DebugRectHighlight()
			{
				Position = rootObject.ScreenSpaceDrawQuad.TopLeft,
				Size = rootObject.ScreenSpaceDrawQuad.BottomRight - rootObject.ScreenSpaceDrawQuad.TopLeft,
			});
		};
		return label;
	}
}
