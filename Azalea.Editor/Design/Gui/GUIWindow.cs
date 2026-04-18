using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using System;
using System.Numerics;

namespace Azalea.Editor.Design.Gui;
public class GUIWindow : BasicWindowContainer
{
	private const float __titleBarHeight = 20;

	private readonly Vector2 _size;

	private readonly GameObject _titleBar;
	private readonly FlexContainer _content;

	internal GUIWindow(string title, Vector2 size)
	{
		Position = new(100);
		Size = _size = size;
		AddRange([
			_titleBar = new WindowTitleBar(title){
				ExpandedChanged = expanded =>{
					if(expanded)
					{
						Add(_content!);
						Size = _size;
					}
					else
					{
						Remove(_content!);
						Size = new(_size.X, __titleBarHeight);
					}
				}
			},
			_content = new FlexContainer(){
				RelativeSizeAxes = Axes.Both,
				NegativeSize = new(0, __titleBarHeight),
				Y = __titleBarHeight,
				BackgroundColor = new Color(21, 22, 23),
				Padding = new(top: 8, right: 5, bottom: 5, left: 5),
				Direction = FlexDirection.Vertical,
				Spacing = new(0, 4)
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

	public GUILabel AddLabel(string text)
	{
		var label = new GUILabel(text);
		_content.Add(label);
		return label;
	}

	public GUICheckbox AddCheckbox(string name, bool @checked = false)
	{
		var checkbox = new GUICheckbox(name, @checked);
		_content.Add(checkbox);
		return checkbox;
	}

	public GUISliderFloat AddSliderFloat(string name,
		float minValue = 0, float maxValue = 1, float initialValue = 0.5f,
		string stringFormat = "0.000", bool continuous = true)
	{
		var sliderFloat = new GUISliderFloat(
			name, minValue, maxValue, initialValue, stringFormat, continuous);
		_content.Add(sliderFloat);
		return sliderFloat;
	}

	protected override bool OnHover(HoverEvent e) => true;
	protected override bool OnClick(ClickEvent e) => true;
	protected override bool OnMouseDown(MouseDownEvent e) => true;

	class WindowTitleBar : FlexContainer
	{
		public Action<bool>? ExpandedChanged
		{
			get => _dropDownArrow.ExpandedChanged;
			set => _dropDownArrow.ExpandedChanged = value;
		}

		private readonly DropDownArrow _dropDownArrow;

		public WindowTitleBar(string title)
		{
			RelativeSizeAxes = Axes.X;
			Height = __titleBarHeight;
			BackgroundColor = new Color(41, 74, 122);

			Direction = FlexDirection.Horizontal;
			Wrapping = FlexWrapping.NoWrapping;
			ContentAlignment = FlexContentAlignment.Center;
			ItemAlignment = FlexItemAlignment.Center;

			AddRange([
				_dropDownArrow = new DropDownArrow()
				{
					Margin = new(5)
				},
				new SpriteText(){
					Text = title,
					Font = GUIConstants.Font
				}
			]);
		}

		class DropDownArrow : Sprite
		{
			public Action<bool>? ExpandedChanged;
			private bool _expanded = true;

			public DropDownArrow()
			{
				Size = new(10);
				Texture = Assets.GetTexture("Gui/arrow-down.png");
				Color = Palette.White;
			}

			protected override bool OnClick(ClickEvent e)
			{
				_expanded = !_expanded;
				Texture = Assets.GetTexture($"Gui/arrow-{(_expanded ? "down" : "up")}.png");

				ExpandedChanged?.Invoke(_expanded);
				return true;
			}
		}
	}
}
