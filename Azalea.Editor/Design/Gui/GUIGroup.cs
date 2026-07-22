using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Editor.Design.Gui;
public class GUIGroup : Composition
{
	private const float __leftMargin = 10;

	private readonly FlexContainer _content;

	internal GUIGroup(string name)
	{
		RelativeSizeAxes = Axes.X;
		Height = 20;

		_content = new FlexContainer()
		{
			RelativeSizeAxes = Axes.X,
			NegativeSize = new(__leftMargin, 0),
			Direction = FlexDirection.Vertical,
			AutoSizeAxes = Axes.Y,
			Position = new(__leftMargin, 24),
			Spacing = new(0, 4)
		};

		base.Add(new GUIWindow.TitleBar(name, 20, GUIConstants.Colors.AccentColor3, false)
		{
			ExpandedChanged = expanded =>
			{
				if (expanded)
				{
					base.Add(_content);
					AutoSizeAxes = Axes.Y;
				}
				else
				{
					base.Remove(_content);
					AutoSizeAxes = Axes.None;
					Height = 20;
				}
			}
		});
	}

	public override void Add(GameObject gameObject) => _content.Add(gameObject);
	public override bool Remove(GameObject gameObject) => _content.Remove(gameObject);
}
