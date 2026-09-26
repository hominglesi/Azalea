using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Utils;
using Azalea.Utils.Proxies;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Editor.Design.Gui;
public class GUIWindow : BasicWindowContainer
{
	private const float __titleBarHeight = 20;

	private readonly Application _app;
	private readonly Vector2 _size;

	private readonly GameObject _titleBar;
	private readonly ScrollableContainer _scrollable;
	private readonly FlexContainer _content;

	internal GUIWindow(Application app, string title, Vector2 position, Vector2 size)
	{
		_app = app;
		Masking = true;
		Position = position;
		Size = _size = size;

		// We don't use AddRange since it wouldn't use the overridden base.Add
		base.Add(_titleBar = new TitleBar(title, __titleBarHeight, new Color(41, 74, 122), true)
		{
			ExpandedChanged = expanded =>
			{
				if (expanded)
				{
					base.Add(_scrollable!);
					Size = _size;
				}
				else
				{
					Remove(_scrollable!);
					Size = new(_size.X, __titleBarHeight);
				}
			}
		});
		base.Add(new CloseButton(__titleBarHeight)
		{
			Origin = Anchor.TopRight,
			Anchor = Anchor.TopRight,
			ClickAction = _ => Hide()
		});
		base.Add(_scrollable = new WindowScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			NegativeSize = new(0, __titleBarHeight),
			Y = __titleBarHeight,
			BackgroundColor = new Color(21, 22, 23),
			Child = _content = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Padding = new(top: 8, right: 5, bottom: 5, left: 5),
				Direction = FlexDirection.Vertical,
				Spacing = new(0, 4)
			}
		});

		AddDragableSurface(_titleBar);
	}

	public static GUIWindow Create(Application app, string title, Vector2 position, Vector2 size)
	{
		var window = new GUIWindow(app, title, position, size);
		app.Game.Add(window);
		return window;
	}

	public override void Add(GameObject gameObject)
	{
		if (_groupStack.Count == 0)
			_content.Add(gameObject);
		else
			_groupStack.Peek().Add(gameObject);
	}

	public void RemoveElement(GameObject obj) => obj.Parent!.Remove(obj);

	private readonly Stack<Composition> _groupStack = [];

	public GUIGroup AddGroup(string name)
	{
		var group = new GUIGroup(name);
		Add(group);
		SelectGroup(group);
		return group;
	}

	public void SelectGroup(Composition group) => _groupStack.Push(group);
	public void FinishGroup() => _groupStack.Pop();

	public GUILabel AddLabel(string text)
	{
		var label = new GUILabel(text);
		Add(label);
		return label;
	}

	public GUILabelContinuous AddLabel(Func<string> textFunction)
	{
		var label = new GUILabelContinuous(textFunction);
		Add(label);
		return label;
	}

	public GUICounter AddCounter(string text, int value)
	{
		var counter = new GUICounter(text, value);
		Add(counter);
		return counter;
	}

	public GUIPropertyLabel<T> AddPropertyLabel<T>(string label, IProxy<T> proxy,
		string? displayString = null)
	{
		var propertyLabel = new GUIPropertyLabel<T>(label, proxy, displayString);
		Add(propertyLabel);
		return propertyLabel;
	}

	public GUIButton AddButton(string text, Action clickAction)
	{
		var button = new GUIButton(text, clickAction);
		Add(button);
		return button;
	}

	public GUICheckbox AddCheckbox(string name, bool @checked = false)
	{
		var checkbox = new GUICheckbox(name, @checked);
		Add(checkbox);
		return checkbox;
	}

	public GUISliderFloat AddSliderFloat(string name,
		float minValue = 0, float maxValue = 1, float initialValue = 0.5f,
		string stringFormat = "0.000", bool continuous = true)
	{
		var sliderFloat = new GUISliderFloat(
			name, minValue, maxValue, initialValue, stringFormat, continuous);
		Add(sliderFloat);
		return sliderFloat;
	}

	public GUIPropertySliderFloat AddPropertySliderFloat(string name, IProxy<float> proxy,
		float minValue = 0, float maxValue = 1, string stringFormat = "0.000")
	{
		var sliderFloat = new GUIPropertySliderFloat(
			name, proxy, minValue, maxValue, stringFormat);
		Add(sliderFloat);
		return sliderFloat;
	}

	public void Hide()
	{
		if (Parent is null)
			return;

		_app.Game.Remove(this);
	}

	public void Show()
	{
		if (Parent is not null)
			return;

		_app.Game.Add(this);
	}

	protected override bool OnHover(HoverEvent e) => true;
	protected override bool OnClick(ClickEvent e) => true;
	protected override bool OnMouseDown(MouseDownEvent e) => true;

	internal class TitleBar : FlexContainer
	{
		public Action<bool>? ExpandedChanged
		{
			get => _dropDownArrow.ExpandedChanged;
			set => _dropDownArrow.ExpandedChanged = value;
		}

		private readonly DropDownArrow _dropDownArrow;
		internal readonly SpriteText Label;

		public TitleBar(string title, float height, Color color, bool expanded)
		{
			RelativeSizeAxes = Axes.X;
			Height = height;
			BackgroundColor = color;

			Direction = FlexDirection.Horizontal;
			Wrapping = FlexWrapping.NoWrapping;
			ContentAlignment = FlexContentAlignment.Center;
			ItemAlignment = FlexItemAlignment.Center;

			AddRange([
				_dropDownArrow = new DropDownArrow(expanded),
				Label = new SpriteText(){
					Text = title,
					Font = GUIConstants.Font
				}
			]);
		}

		class DropDownArrow : Composition
		{
			public Action<bool>? ExpandedChanged;
			private bool _expanded = true;

			private readonly Sprite _sprite;

			public DropDownArrow(bool expanded)
			{
				_expanded = expanded;

				Size = new(20);
				Add(_sprite = new Sprite()
				{
					Texture = Assets.GetTexture($"Gui/arrow-{(_expanded ? "down" : "up")}.png"),
					Anchor = Anchor.Center,
					Origin = Anchor.Center
				});
			}

			protected override bool OnClick(ClickEvent e)
			{
				_expanded = !_expanded;
				_sprite.Texture = Assets.GetTexture($"Gui/arrow-{(_expanded ? "down" : "up")}.png");

				ExpandedChanged?.Invoke(_expanded);
				return true;
			}
		}
	}

	internal class CloseButton : Composition
	{
		public CloseButton(float size)
		{
			Size = new(size);
			Add(new Sprite()
			{
				Texture = Assets.GetTexture($"Gui/cross.png"),
				Origin = Anchor.Center,
				Anchor = Anchor.Center
			});
		}

		protected override bool OnHover(HoverEvent e)
		{
			BackgroundColor = new Color(255, 255, 255, 60);
			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			BackgroundColor = new Color(255, 255, 255, 0);
		}
	}

	internal class WindowScrollableContainer : ScrollableContainer
	{
		protected override Slider CreateSlider()
			=> new WindowScrollableSlider()
			{
				Origin = Anchor.TopRight,
				Anchor = Anchor.TopRight,
				Direction = SliderDirection.Vertical,
				RelativeSizeAxes = Axes.Y,
				Height = 1f,
				Width = 15,
			};

		private class WindowScrollableSlider : Slider
		{
			protected override GameObject CreateBody()
				=> new Box()
				{
					Color = GUIConstants.Colors.BackgroundDarkest,
					RelativeSizeAxes = Axes.Both
				};

			protected override GameObject CreateHead()
				=> new Box()
				{
					Origin = Anchor.Center,
					Color = GUIConstants.Colors.SliderBody,
					RelativeSizeAxes = Axes.Y,
					Size = new(9, 0.2f)
				};
		}
	}
}
