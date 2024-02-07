using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Extentions.EnumExtentions;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using System;

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

		if (rootObject is Composition comp)
		{
			rootObject.Invalidated += (_, invalidation) =>
			{
				if (invalidation.HasFlagFast(Invalidation.Presence))
				{
					if (_childCount != comp.InternalChildren.Count)
						toggleChildren(_checkbox.Checked);
				}
			};
		}

		toggleChildren(false);
	}

	private void toggleChildren(bool shown)
	{
		_content.Clear();
		_content.Add(createLabel(_rootObject));

		if (shown)
		{
			if (_rootObject is CompositeGameObject comp)
			{
				foreach (var child in comp.InternalChildren)
				{
					var childGraph = new DebugSceneGraph(child);
					childGraph.ObjectSelected += ObjectSelected!.Invoke;
					_content.Add(childGraph);
				}
			}
		}

		if (_rootObject is Composition composition)
		{
			_childCount = composition.InternalChildren.Count;
		}
	}

	public event Action<GameObject>? ObjectSelected;

	private SpriteText createLabel(GameObject rootObject)
	{
		var label = new SpriteText()
		{
			Text = _rootObject.GetType().Name,
		};
		label.Click += (_) => ObjectSelected?.Invoke(rootObject);
		return label;
	}
}
