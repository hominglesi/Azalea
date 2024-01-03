using Azalea.Design.Containers;
using Azalea.Design.Containers.Text;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;

namespace Azalea.VisualTests;
public class FlexTest : TestScene
{
	private FlexContainer _flex;
	private Box _firstChild;

	private TextContainer _textFlex;

	public FlexTest()
	{
		AddRange(new GameObject[]
		{
			new Composition()
			{
				RelativeSizeAxes = Axes.Both,
				Size = new(1, 0.25f),
				Children = new GameObject[]
				{
					new Box()
					{
						RelativeSizeAxes = Axes.Both,
						Color = Palette.White,
					},
					_flex = createFirstFlex()
				}
			},
			_textFlex = new TextContainer()
			{
				Width = 1000,
				Height = 1000,
			}
		});
	}

	private FlexContainer createFirstFlex()
	{
		return new FlexContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Wrapping = FlexWrapping.Wrap,
			Children = new GameObject[]
			{
				_firstChild = new Box()
				{
					Color = Palette.Flowers.Azalea,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.2f, 0.5f)
				},
				new Box()
				{
					Color = Palette.Aqua,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.2f, 0.5f)
				},
				new Box()
				{
					Color = Palette.Black,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.6f, 0.5f),
				},
				new Box()
				{
					Color = Palette.Black,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.2f, 0.5f),
				}
			}
		};
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.P).Down)
		{
			_firstChild.Size = new(_firstChild.Size.X + 0.10f, _firstChild.Size.Y);
		}

		var tempChildren = new GameObject[_flex.Children.Count];
		for (int i = 0; i < tempChildren.Length; i++)
		{
			tempChildren[i] = _flex.Children[i];
		}
		_flex.Clear();
		_flex.AddRange(tempChildren);

		_textFlex.Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum";
	}
}
