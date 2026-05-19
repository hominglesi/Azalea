using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Editor.Design.Gui;
public class GUIWindow : BasicWindowContainer
{
	private const float __titleBarHeight = 20;

	private readonly Vector2 _size;

	private readonly GameObject _titleBar;
	private readonly ScrollableContainer _scrollable;
	private readonly FlexContainer _content;

	internal GUIWindow(string title, Vector2 size)
	{
		Masking = true;
		Position = new(100);
		Size = _size = size;
		AddRange([
			_titleBar = new TitleBar(title, __titleBarHeight, new Color(41, 74, 122), true){
				ExpandedChanged = expanded =>{
					if(expanded)
					{
						Add(_scrollable!);
						Size = _size;
					}
					else
					{
						Remove(_scrollable!);
						Size = new(_size.X, __titleBarHeight);
					}
				}
			},
			_scrollable = new WindowScrollableContainer(){
				RelativeSizeAxes = Axes.Both,
				NegativeSize = new(0, __titleBarHeight),
				Y = __titleBarHeight,
				BackgroundColor = new Color(21, 22, 23),
				Child = _content = new FlexContainer(){
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y,
					Padding = new(top: 8, right: 5, bottom: 5, left: 5),
					Direction = FlexDirection.Vertical,
					Spacing = new(0, 4)
				}
			}

		]);

		AddDragableSurface(_titleBar);
	}

	public static GUIWindow Create(string title, Vector2 size)
	{
		// EditorWrapper might have not been referenced yet 
		// so the resources would not be loaded yet
		_ = EditorWrapper.Instance;

		var window = new GUIWindow(title, size);
		EditorWrapper.Instance.Add(window);
		return window;
	}

	private void addToWindow(GameObject obj)
	{
		if (_groupStack.Count == 0)
			_content.Add(obj);
		else
			_groupStack.Peek().Add(obj);
	}

	private readonly Stack<GUIGroup> _groupStack = [];

	public GUIGroup AddGroup(string name)
	{
		var group = new GUIGroup(name);
		addToWindow(group);
		_groupStack.Push(group);
		return group;
	}

	public void FinishGroup()
	{
		_groupStack.Pop();
	}

	public GUILabel AddLabel(string text)
	{
		var label = new GUILabel(text);
		addToWindow(label);
		return label;
	}

	public GUILabelContinuous AddLabel(Func<string> textFunction)
	{
		var label = new GUILabelContinuous(textFunction);
		addToWindow(label);
		return label;
	}

	public GUICheckbox AddCheckbox(string name, bool @checked = false)
	{
		var checkbox = new GUICheckbox(name, @checked);
		addToWindow(checkbox);
		return checkbox;
	}

	public GUISliderFloat AddSliderFloat(string name,
		float minValue = 0, float maxValue = 1, float initialValue = 0.5f,
		string stringFormat = "0.000", bool continuous = true)
	{
		var sliderFloat = new GUISliderFloat(
			name, minValue, maxValue, initialValue, stringFormat, continuous);
		addToWindow(sliderFloat);
		return sliderFloat;
	}

	public void Hide()
	{
		if (Parent is null)
			return;

		EditorWrapper.Instance.Remove(this);
	}

	public void Show()
	{
		if (Parent is not null)
			return;

		EditorWrapper.Instance.Add(this);
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
				new SpriteText(){
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
					Size = new(10),
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
