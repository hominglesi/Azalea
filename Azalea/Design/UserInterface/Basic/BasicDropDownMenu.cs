using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Design.UserInterface.Basic;
public class BasicDropDownMenu : DropDownMenu<string>
{
	private static Color _accentColor = Palette.Pink;
	private static Color _hoverColor = new Color(255, 230, 240);

	private readonly HollowBox _outline;
	private readonly Box _background;
	private readonly SpriteText _label;
	private readonly Sprite _arrow;

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

		Add(_label = new SpriteText()
		{
			Text = "Choose an item...",
			Font = FontUsage.Default.With(size: 32),
			Color = Palette.Blue,
			Anchor = Anchor.CenterLeft,
			Origin = Anchor.CenterLeft,
			X = 10
		});

		Add(_arrow = new Sprite()
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
		_arrow.Scale = Vector2.One;
	}

	public override void Expand()
	{
		base.Expand();
		_arrow.Scale = new(1, -1);
	}

	public void AddOption(string name, string? value = null)
	{
		value ??= name;

		var expanded = (BasicDropDownExpanded)ExpandedSegment;
		var item = new BasicDropDownItem(name, value)
		{
			ClickAction = _ => updateSelected(name, value)
		};
		expanded.AddItem(item);
	}

	public void SelectOption(string value)
	{
		foreach (var item in ((BasicDropDownExpanded)ExpandedSegment).GetItems())
		{
			if (item.Value == value)
			{
				updateSelected(item.Label, item.Value);
				return;
			}
		}
	}

	private void updateSelected(string label, string value)
	{
		_label.Text = label;
		SelectedValue = value;
	}

	protected override DropDownExpanded CreateExpandedSegment()
		=> new BasicDropDownExpanded();

	public class BasicDropDownExpanded : DropDownExpanded
	{
		private HollowBox _outline;
		private Box _shadow;
		private FlexContainer _inner;
		public BasicDropDownExpanded()
		{
			Anchor = Anchor.BottomLeft;
			Y = 8;

			RelativeSizeAxes = Axes.X;
			AutoSizeAxes = Axes.Y;

			Add(_shadow = new Box()
			{
				RelativeSizeAxes = Axes.Both,
				Color = new Color(0, 0, 0, 100),
				Position = new(4)
			});

			Add(_inner = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Vertical,
			});

			Add(_outline = new HollowBox()
			{
				RelativeSizeAxes = Axes.Both,
				Color = _accentColor
			});
		}

		public void AddItem(GameObject obj)
			=> _inner.Add(obj);

		public IEnumerable<BasicDropDownItem> GetItems()
		{
			foreach (var item in _inner.Children)
				yield return (BasicDropDownItem)item;
		}
	}

	public class BasicDropDownItem : Composition
	{
		public string Label;
		public string Value;
		private SpriteText _label;
		public BasicDropDownItem(string label, string value)
		{
			Label = label;
			Value = value;

			RelativeSizeAxes = Axes.X;
			Height = 55;
			BackgroundColor = Palette.White;

			Add(_label = new SpriteText()
			{
				Text = label,
				Font = FontUsage.Default.With(size: 32),
				Color = Palette.Blue,
				Anchor = Anchor.CenterLeft,
				Origin = Anchor.CenterLeft,
				X = 10
			});
		}

		protected override bool OnHover(HoverEvent e)
		{
			BackgroundColor = _hoverColor;
			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			BackgroundColor = Palette.White;
		}
	}
}
