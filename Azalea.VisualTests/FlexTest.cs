﻿using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Shapes;
using Azalea.Inputs;

namespace Azalea.VisualTests;
public class FlexTest : TestScene
{
	private FlexContainer _flex;
	private Box _firstChild;

	public FlexTest()
	{
		AddRange(new GameObject[]
		{
			new Container()
			{
				RelativeSizeAxes = Axes.Both,
				Size = new(1, 0.25f),
				Children = new GameObject[]
				{
					new Box()
					{
						RelativeSizeAxes = Axes.Both,
						Color = Color.White,
					},
					_flex = createFirstFlex()
				}
			},
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
					Color = Color.Azalea,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.2f, 0.5f)
				},
				new Box()
				{
					Color = Color.Aqua,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.2f, 0.5f)
				},
				new Box()
				{
					Color = Color.Black,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.6f, 0.5f),
				},
				new Box()
				{
					Color = Color.Black,
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
	}
}