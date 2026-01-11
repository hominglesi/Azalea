using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using System.Collections.Generic;

namespace Azalea.Design.Controls;
public class DropDownMenuControl : AbstractDropDownMenu<string>
{
	private readonly SpriteText _selectedItemText;

	private readonly FlexContainer _expandedPart;

	public DropDownMenuControl()
	{
		Size = new(1, ControlConstants.InputControlHeight);
		RelativeSizeAxes = Axes.X;

		BorderColor = ControlConstants.DarkControlColor;
		BorderThickness = 1;

		AddRange([
			_selectedItemText = new SpriteText()
			{
				Font = ControlConstants.NormalFont,
				Color = ControlConstants.DarkTextColor,
				X = 12,
				Origin = Anchor.CenterLeft,
				Anchor = Anchor.CenterLeft,
				Text = "Select Item..."
			},
			new SpriteText(){
				Font = ControlConstants.NormalFont,
				Color = ControlConstants.DarkTextColor,
				X = -12,
				Origin = Anchor.CenterRight,
				Anchor = Anchor.CenterRight,
				Text = "V"
			}
		]);

		_expandedPart = new FlexContainer()
		{
			RelativeSizeAxes = Axes.X,
			AutoSizeAxes = Axes.Y,
			BorderColor = ControlConstants.DarkControlColor,
			BorderThickness = 1,
			Direction = FlexDirection.Vertical,
			BackgroundColor = Palette.White,
			Y = ControlConstants.InputControlHeight
		};

		OnSelectedChanged += item => _selectedItemText.Text = item ?? "Select Item...";
	}

	protected override GameObject CreateExpandedSegment()
		=> _expandedPart;

	private readonly List<MenuOption> _options = [];

	protected override void AddOptionInternal(string item)
	{
		var option = new MenuOption(item, this);

		_options.Add(option);
		_expandedPart.Add(option);
	}

	protected override void ClearOptionsInternal()
	{
		_options.Clear();
		_expandedPart.Clear();
	}

	protected override void RemoveOptionInternal(string item)
	{
		MenuOption? option = null;

		foreach (var opt in _options)
			if (opt.Value == item)
			{
				option = opt;
				break;
			}

		if (option is null)
			return;

		_options.Remove(option);
		_expandedPart.Remove(option);
	}

	protected override void ExpandImplementation()
	{
		Add(ExpandedSegment);
	}

	protected override void ContractImplementation()
	{
		Remove(ExpandedSegment);
	}

	private class MenuOption : Composition
	{
		public readonly string Value;
		private readonly DropDownMenuControl _parent;

		public MenuOption(string value, DropDownMenuControl parent)
		{
			Value = value;
			_parent = parent;

			Size = new(1, ControlConstants.InputControlHeight);
			RelativeSizeAxes = Axes.X;

			Add(new SpriteText()
			{
				Font = ControlConstants.NormalFont,
				Color = ControlConstants.DarkTextColor,
				Text = value,
				Origin = Anchor.CenterLeft,
				Anchor = Anchor.CenterLeft,
				X = 12
			});
		}

		protected override bool OnClick(ClickEvent e)
		{
			_parent.SelectedValue = Value;
			return true;
		}

		protected override bool OnHover(HoverEvent e)
		{
			BackgroundColor = new Color(221, 224, 227);
			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			BackgroundColor = Palette.White;
		}
	}
}
