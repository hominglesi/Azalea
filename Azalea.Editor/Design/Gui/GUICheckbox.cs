using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using System;

namespace Azalea.Editor.Design.Gui;
public class GUICheckbox : FlexContainer
{
	private readonly Sprite _checkmarkSprite;

	private bool _checked;

	internal GUICheckbox(string name, bool @checked)
	{
		_checked = @checked;

		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		Direction = FlexDirection.Horizontal;
		Wrapping = FlexWrapping.NoWrapping;
		ItemAlignment = FlexItemAlignment.Center;
		Spacing = new(5);

		AddRange([
			new Composition(){
				Size = new(19),
				Children = [
					new Box(){
						RelativeSizeAxes = Axes.Both,
						Color = GUIConstants.Colors.AccentColor,
						ClickAction = _ => {
							_checked = !_checked;
							_checkmarkSprite!.Alpha = _checked? 1 : 0;
							_checkedChanged?.Invoke(_checked);
						}
					},
					_checkmarkSprite = new Sprite(){
						Texture = Assets.GetTexture("Gui/checkmark.png"),
						Origin = Anchor.Center,
						Anchor = Anchor.Center,
						Alpha = _checked ? 1 : 0
					}
				]
			},
			new SpriteText(){
				Text = name,
				Font = GUIConstants.Font
			}
		]);
	}

	private Action<bool>? _checkedChanged;
	public void OnCheckedChanged(Action<bool> checkedChanged)
		=> _checkedChanged = checkedChanged;
}
