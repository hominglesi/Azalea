using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Utils;
using System;

namespace Azalea.VisualTests;
public class FlexContainerTest : TestScene
{
	private BasicButton _createSampleButton;

	private BasicDropDownMenu _directionMenu;
	private BasicDropDownMenu _wrappingMenu;
	private BasicDropDownMenu _justificationMenu;
	private BasicDropDownMenu _itemAlignmentMenu;
	private BasicDropDownMenu _contentAlignmentMenu;
	private BasicDropDownMenu _spacingHorizontalMenu;
	private BasicDropDownMenu _spacingVerticalMenu;

	public FlexContainerTest()
	{
		_createSampleButton = AddButton("Create sample flex container");
		_createSampleButton.ClickAction = _ => createSampleFlex();

		_directionMenu = AddDropdownMenu();
		_directionMenu.AddOption("Horizontal");
		_directionMenu.AddOption("HorizontalReverse");
		_directionMenu.AddOption("Vertical");
		_directionMenu.AddOption("VerticalReverse");
		_directionMenu.SelectOption(0);
		_directionMenu.OnSelectedChanged += direction =>
		{
			if (_flex is null || direction is null) return;
			_flex.Direction = Enum.Parse<FlexDirection>(direction);
		};

		_wrappingMenu = AddDropdownMenu();
		_wrappingMenu.AddOption("NoWrapping");
		_wrappingMenu.AddOption("Wrap");
		_wrappingMenu.AddOption("WrapReverse");
		_wrappingMenu.SelectOption(0);
		_wrappingMenu.OnSelectedChanged += wrapping =>
		{
			if (_flex is null || wrapping is null) return;
			_flex.Wrapping = Enum.Parse<FlexWrapping>(wrapping);
		};

		_justificationMenu = AddDropdownMenu();
		_justificationMenu.AddOption("Start");
		_justificationMenu.AddOption("End");
		_justificationMenu.AddOption("Center");
		_justificationMenu.AddOption("SpaceBetween");
		_justificationMenu.AddOption("SpaceAround");
		_justificationMenu.AddOption("SpaceEvenly");
		_justificationMenu.SelectOption(0);
		_justificationMenu.OnSelectedChanged += justification =>
		{
			if (_flex is null || justification is null) return;
			_flex.Justification = Enum.Parse<FlexJustification>(justification);
		};

		_itemAlignmentMenu = AddDropdownMenu();
		_itemAlignmentMenu.AddOption("Start");
		_itemAlignmentMenu.AddOption("End");
		_itemAlignmentMenu.AddOption("Center");
		_itemAlignmentMenu.SelectOption(0);
		_itemAlignmentMenu.OnSelectedChanged += itemAlignment =>
		{
			if (_flex is null || itemAlignment is null) return;
			_flex.Alignment = Enum.Parse<FlexAlignment>(itemAlignment);
		};

		_contentAlignmentMenu = AddDropdownMenu();
		_contentAlignmentMenu.AddOption("Start");
		_contentAlignmentMenu.AddOption("End");
		_contentAlignmentMenu.AddOption("Center");
		_contentAlignmentMenu.AddOption("SpaceBetween");
		_contentAlignmentMenu.AddOption("SpaceAround");
		_contentAlignmentMenu.AddOption("SpaceEvenly");
		_contentAlignmentMenu.SelectOption(0);
		_contentAlignmentMenu.OnSelectedChanged += contentAlignment =>
		{
			if (_flex is null || contentAlignment is null) return;
			_flex.ContentAlignment = Enum.Parse<FlexContentAlignment>(contentAlignment);
		};

		_spacingHorizontalMenu = AddDropdownMenu();
		_spacingHorizontalMenu.AddOption("0px", "0");
		_spacingHorizontalMenu.AddOption("2px", "2");
		_spacingHorizontalMenu.AddOption("5px", "5");
		_spacingHorizontalMenu.AddOption("10px", "10");
		_spacingHorizontalMenu.SelectOption(0);
		_spacingHorizontalMenu.OnSelectedChanged += spacingHorizontal =>
		{
			if (_flex is null || spacingHorizontal is null) return;
			_flex.Spacing = new(int.Parse(spacingHorizontal), _flex.Spacing.Y);
		};

		_spacingVerticalMenu = AddDropdownMenu();
		_spacingVerticalMenu.AddOption("0px", "0");
		_spacingVerticalMenu.AddOption("2px", "2");
		_spacingVerticalMenu.AddOption("5px", "5");
		_spacingVerticalMenu.AddOption("10px", "10");
		_spacingVerticalMenu.SelectOption(0);
		_spacingVerticalMenu.OnSelectedChanged += spacingVertical =>
		{
			if (_flex is null || spacingVertical is null) return;
			_flex.Spacing = new(_flex.Spacing.X, int.Parse(spacingVertical));
		};
	}

	private FlexContainer _flex;

	private void createSampleFlex()
	{
		if (_flex is not null)
			Remove(_flex);

		Add(_flex = new FlexContainer()
		{
			Size = new(500),
			Origin = Anchor.Center,
			Anchor = Anchor.Center,
			BackgroundColor = Palette.White,
			Direction = FlexDirection.Horizontal,
			Wrapping = FlexWrapping.Wrap,
			ContentAlignment = FlexContentAlignment.Start
		});

		_directionMenu.SelectOption("Horizontal");
		_wrappingMenu.SelectOption("Wrap");
		_contentAlignmentMenu.SelectOption("Start");

		for (int i = 0; i < 20; i++)
		{
			var color = Rng.Color();

			_flex.Add(new Composition()
			{
				Size = Rng.Vector2(new(50, 100)),
				BackgroundColor = color,
				Child = new SpriteText()
				{
					Origin = Anchor.Center,
					Anchor = Anchor.Center,
					Text = (i + 1).ToString(),
					Color = color.Brightness > 127 ? Palette.Black : Palette.White,
				}
			});
		}
	}
}
