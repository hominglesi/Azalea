using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.UserInterface;

namespace Azalea.Debugging;
public class DebugSceneGraph : FlexContainer
{
	private Checkbox _checkbox;
	private Composition _content;

	private GameObject _rootObject;

	public DebugSceneGraph(GameObject rootObject)
	{
		_rootObject = rootObject;
		Wrapping = FlexWrapping.NoWrapping;

		if (_rootObject is Composition)
		{
			Add(_checkbox = new Checkbox()
			{
				Size = new(25, 25)
			});
			_checkbox.Toggled += toggleChildren;
		}

		Add(new Composition()
		{
			Child = _content = new FlexContainer()
			{
				Direction = FlexDirection.Vertical,
				Wrapping = FlexWrapping.NoWrapping,
				Child = createLabel()
			}
		});

		if (_rootObject is not Composition)
		{
			_content.Margin = new(0, 0, 0, 25);
		}
	}

	private void toggleChildren(bool shown)
	{
		_content.Clear();
		var label = createLabel();
		_content.Add(label);
		_content.Height += label.Height;

		if (shown)
		{
			if (_rootObject is Composition comp)
			{
				foreach (var child in comp.Children)
				{
					var childGraph = new DebugSceneGraph(child);
					_content.Add(childGraph);
					_content.Height += childGraph.Height;
				}
			}
		}

		Height = _content.Height;
	}

	private SpriteText createLabel() => new()
	{
		Text = _rootObject.GetType().Name
	};
}
