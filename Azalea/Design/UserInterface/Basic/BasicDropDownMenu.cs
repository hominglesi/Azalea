using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Design.UserInterface.Basic;
public class BasicDropDownMenu : DropDownMenu<string>
{
	public readonly Sprite Arrow;
	public readonly SpriteText Label;
	private readonly HollowBox _outline;
	private readonly Box _background;

	#region Customisation

	private Color _accentColor = Palette.Pink;
	public Color AccentColor
	{
		get => _accentColor;
		set
		{
			if (_accentColor == value)
				return;

			_accentColor = value;

			_outline.Color = _accentColor;
			_basicExpandedSegment.Outline.Color = _accentColor;
		}
	}

	private float _itemHeight = 55;
	public float ItemHeight
	{
		get => _itemHeight;
		set
		{
			if (_itemHeight == value)
				return;

			_itemHeight = value;

			foreach (var item in _basicExpandedSegment.GetItems())
				item.Height = _itemHeight;
		}
	}

	private FontUsage _itemFont = FontUsage.Default.With(size: 20);
	public FontUsage ItemFont
	{
		get => _itemFont;
		set
		{
			if (_itemFont == value)
				return;

			_itemFont = value;

			foreach (var item in _basicExpandedSegment.GetItems())
				item.Label.Font = _itemFont;
		}
	}

	private Color _itemHoverColor = new(255, 230, 240);
	public Color ItemHoverColor
	{
		get => _itemHoverColor;
		set
		{
			if (_itemHoverColor == value)
				return;

			_itemHoverColor = value;
		}
	}

	private Color _itemTextColor = Palette.Blue;
	public Color ItemTextColor
	{
		get => _itemTextColor;
		set
		{
			if (_itemTextColor == value)
				return;

			_itemTextColor = value;

			foreach (var item in _basicExpandedSegment.GetItems())
				item.Label.Color = _itemTextColor;
		}
	}

	private float _dropDownOffset = 8;
	public float DropDownOffset
	{
		get => _dropDownOffset;
		set
		{
			if (_dropDownOffset == value)
				return;

			_dropDownOffset = value;

			_basicExpandedSegment.Y = _dropDownOffset;
		}
	}

	#endregion

	public BasicDropDownMenu()
	{
		Size = new(360, 55);

		Add(_background = new Box()
		{
			RelativeSizeAxes = Axes.Both,
			Color = Palette.White
		});

		Add(_outline = new HollowBox()
		{
			RelativeSizeAxes = Axes.Both,
			Color = _accentColor,
		});

		Add(Label = new SpriteText()
		{
			Font = _itemFont,
			Text = "Choose an item...",
			Color = Palette.Blue,
			Anchor = Anchor.CenterLeft,
			Origin = Anchor.CenterLeft,
			X = 10
		});

		Add(Arrow = new Sprite()
		{
			Anchor = Anchor.CenterRight,
			Origin = Anchor.CenterRight,
			X = -10,
			Texture = Assets.GetTexture("Textures/vertical-arrow.png"),
			Color = Palette.Blue
		});
	}

	protected override bool OnClick(ClickEvent e)
	{
		if (IsExpanded)
			Contract();
		else
			Expand();

		return true;
	}

	public override void Contract()
	{
		base.Contract();
		Arrow.Scale = Vector2.One;
	}

	public override void Expand()
	{
		base.Expand();
		Arrow.Scale = new(1, -1);
	}

	public void AddOption(string name, string? value = null)
	{
		value ??= name;

		var item = new BasicDropDownItem(this, name, value);
		item.OnSelected += () => updateSelected(name, value);
		_basicExpandedSegment.AddItem(item);
	}

	public void ClearOptions()
		=> _basicExpandedSegment.ClearItems();

	public void SelectOption(string value)
	{
		foreach (var item in _basicExpandedSegment.GetItems())
		{
			if (item.Value == value)
			{
				updateSelected(item.Name, item.Value);
				return;
			}
		}
	}

	public void SelectOption(int index)
	{
		var item = _basicExpandedSegment.GetItems().GetEnumerator();
		int i = 0;
		while (item.MoveNext())
		{
			if (i++ == index)
			{
				var option = item.Current;
				updateSelected(option.Name, option.Value);
				return;
			}
		}
	}

	private void updateSelected(string label, string value)
	{
		Label.Text = label;
		SelectedValue = value;
	}

	protected override DropDownExpanded CreateExpandedSegment()
		=> new BasicDropDownExpanded(this);

	private BasicDropDownExpanded _basicExpandedSegment
	{ get => (BasicDropDownExpanded)ExpandedSegment; }

	public class BasicDropDownExpanded : DropDownExpanded
	{
		private BasicDropDownMenu _parentMenu;

		public readonly HollowBox Outline;
		private readonly Box _shadow;
		private readonly FlexContainer _inner;
		public BasicDropDownExpanded(BasicDropDownMenu parentMenu)
		{
			_parentMenu = parentMenu;

			Anchor = Anchor.BottomLeft;
			Y = parentMenu.DropDownOffset;

			RelativeSizeAxes = Axes.X;
			AutoSizeAxes = Axes.Y;

			Add(_shadow = new Box()
			{
				RelativeSizeAxes = Axes.Both,
				Color = new Color(0, 0, 0, 100),
				Position = new(4),
				IgnoredForAutoSizeAxes = Axes.Both
			});

			Add(_inner = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Vertical,
			});

			Add(Outline = new HollowBox()
			{
				RelativeSizeAxes = Axes.Both,
				Color = _parentMenu.AccentColor,
				IgnoredForAutoSizeAxes = Axes.Both
			});
		}

		public void AddItem(GameObject obj)
			=> _inner.Add(obj);

		public IEnumerable<BasicDropDownItem> GetItems()
		{
			foreach (var item in _inner.Children)
				yield return (BasicDropDownItem)item;
		}

		public void ClearItems()
			=> _inner.Clear();
	}

	public class BasicDropDownItem : Composition
	{
		public Action? OnSelected;

		private BasicDropDownMenu _parentMenu;

		public string Name;
		public string Value;

		public readonly SpriteText Label;

		public BasicDropDownItem(BasicDropDownMenu parentMenu, string name, string value)
		{
			_parentMenu = parentMenu;

			Name = name;
			Value = value;

			RelativeSizeAxes = Axes.X;
			Height = _parentMenu.ItemHeight;
			BackgroundColor = Palette.White;

			Add(Label = new SpriteText()
			{
				Text = Name,
				Font = parentMenu.ItemFont,
				Color = _parentMenu.ItemTextColor,
				Anchor = Anchor.CenterLeft,
				Origin = Anchor.CenterLeft,
				X = 10
			});
		}

		protected override bool OnHover(HoverEvent e)
		{
			BackgroundColor = _parentMenu.ItemHoverColor;
			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			BackgroundColor = Palette.White;
		}

		protected override bool OnClick(ClickEvent e)
		{
			OnSelected?.Invoke();
			return true;
		}
	}
}
