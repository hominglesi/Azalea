using Azalea.Design.Containers;
using Azalea.Design.Schemes;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using System;
using System.Collections.Generic;

namespace Azalea.VisualTests;
public class SchemesTest : TestScene
{
	private ScrollableContainer _container;

	public SchemesTest()
	{
		var schemeFormatter = new SchemeFormatter();
		schemeFormatter.AddScheme("Post", createPost);
		schemeFormatter.AddScheme("Section", createSection);
		schemeFormatter.AddScheme("Title", createTitle);
		schemeFormatter.AddScheme("SubTitle", createSubTitle);
		schemeFormatter.AddScheme("Paragraph", createParagraph);
		schemeFormatter.AddScheme("LineBreak", createLineBreak);
		schemeFormatter.AddScheme("UnorderedList", createUnorderedList);
		schemeFormatter.AddScheme("ListItem", createListItem);
		schemeFormatter.AddScheme("Image", createImage);


		Add(_container = new ScrollableContainer()
		{
			RelativeSizeAxes = Axes.Y,
			Size = new(1200, 1),
			BackgroundColor = new Color(24, 19, 38),
			Anchor = Anchor.Center,
			Origin = Anchor.Center
		});

		_container.ScrollBar.Head.Color = new Color(255, 255, 255);
		_container.Add(schemeFormatter.Format(Assets.GetText("Xml/SchemeTest.xml")!));
	}

	private GameObject createPost(List<object> content)
	{
		var container = new FlexContainer()
		{
			Anchor = Anchor.TopCenter,
			Origin = Anchor.TopCenter,
			Width = 1000,
			AutoSizeAxes = Axes.Y,
			Direction = FlexDirection.Vertical,
			Wrapping = FlexWrapping.NoWrapping
		};

		container.Add(new Box()
		{
			Size = new(1, 50),
			Alpha = 0.01f
		});

		foreach (var item in content)
		{
			if (item is GameObject obj)
				container.Add(obj);
		}

		container.Add(new Box()
		{
			Size = new(1, 50),
			Alpha = 0.01f
		});

		return container;
	}

	private GameObject createTitle(List<object> content)
	{
		if (content.Count != 1 && content[0] is not string)
			throw new Exception("This tag only accepts text");

		string text = (string)content[0];

		return new SpriteText()
		{
			Margin = new(30, 0, 0, 40),
			Text = text,
			Font = FontUsage.Default.With(size: 42)
		};
	}

	private GameObject createSubTitle(List<object> content)
	{
		if (content.Count != 1 && content[0] is not string)
			throw new Exception("This tag only accepts text");

		string text = (string)content[0];

		return new SpriteText()
		{
			Text = text,
			Margin = new(30, 0, 0, 40),
			Font = FontUsage.Default.With(size: 32)
		};
	}

	private GameObject createLineBreak(List<object> content)
	{
		return new Box
		{
			Size = new(900, 1),
			Color = new Color(70, 66, 81),
			Margin = new(40, 0, 0, 40),
		};
	}

	private GameObject createUnorderedList(List<object> content)
	{
		var container = new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
			Margin = new(30, 0, 0, 40),
			Width = 900,
			AutoSizeAxes = Axes.Y,
		};

		foreach (var item in content)
		{
			if (item is not GameObject obj)
				continue;

			container.Add(new Composition()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Children = new GameObject[]
				{
					new Box()
					{
						Size = new(5),
						Margin = new(10)
					},
					obj
				}
			});

			obj.Width = 875;
			obj.Margin = new(0, 0, 0, 25);
		}

		return container;
	}

	private GameObject createImage(List<object> content)
	{
		if (content.Count != 1 && content[0] is not string)
			throw new Exception("This tag only accepts text");

		string text = (string)content[0];

		var texture = Assets.GetTexture(text);
		var aspectRatio = texture.Height / (float)texture.Width;
		return new Sprite()
		{
			Size = new(900, 900 * aspectRatio),
			Texture = texture,
			Margin = new(20, 0, 0, 40),
		};
	}

	private GameObject createListItem(List<object> content)
	{
		if (content.Count != 1 && content[0] is not string)
			throw new Exception("This tag only accepts text");

		string text = (string)content[0];

		return new TextContainer(t => t.Font = FontUsage.Default.With(size: 24))
		{
			AutoSizeAxes = Axes.Y,
			Text = text
		};
	}

	private GameObject createParagraph(List<object> content)
	{
		if (content.Count != 1 && content[0] is not string)
			throw new Exception("This tag only accepts text");

		string text = (string)content[0];

		return new TextContainer(t => t.Font = FontUsage.Default.With(size: 24))
		{
			Text = text,
			Margin = new(30, 0, 0, 40),
			Width = 900,
			AutoSizeAxes = Axes.Y
		};
	}

	private GameObject createSection(List<object> content)
	{
		var container = new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
			Margin = new(20, 0, 0, 0),
			AutoSizeAxes = Axes.Y,
			RelativeSizeAxes = Axes.X,
			BorderThickness = new(1),
			BorderColor = new Color(70, 66, 81),
		};

		foreach (var item in content)
		{
			if (item is GameObject obj)
				container.Add(obj);
		}

		container.Add(new Box()
		{
			Size = new(1, 30),
			Alpha = 0.01f
		});

		return container;
	}
}
