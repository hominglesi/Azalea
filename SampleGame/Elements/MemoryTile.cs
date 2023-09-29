using Azalea.Design.Compositions;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.Graphics.UserInterface;
using Azalea.IO.Assets;
using System;
using System.Numerics;

namespace SampleGame.Elements;

public class MemoryTile : Composition
{
	public const string DefaultTexturePath = @"blank.png";

	public bool IsShown = false;
	public string TextureName;

	private readonly Button _button;
	private readonly Sprite _sprite;
	private readonly Texture _texture;
	private readonly SpriteText _number;

	public Action? Action;

	public MemoryTile(Texture texture, int index)
	{
		_texture = texture;
		TextureName = _texture.AssetName;
		Padding = new Boundary(3);

		RelativeSizeAxes = Axes.Both;

		AddRange(new GameObject[]{
			_button = new Button()
			{
				Action = () => Action?.Invoke(),
				RelativeSizeAxes = Axes.Both,
				Children = new GameObject[]
				{
					_sprite = new Sprite()
					{
						Texture = Assets.GetTexture(DefaultTexturePath),
						RelativeSizeAxes = Axes.Both,
						Size = new Vector2(1f, 1f)
					},
					_number = new SpriteText()
					{
						Text = index.ToString(),
						Origin = Anchor.Center,
						Position = new Vector2(0.5f, 0.5f),
						RelativePositionAxes = Axes.Both,
						Font = new FontUsage(family: "Roboto", size: 140, weight: "Medium"),
						Color = new Color(201, 132, 146)
					}
				}
			} });
	}

	public void Show()
	{
		if (IsShown) return;
		IsShown = true;

		_sprite.Texture = _texture;
		_number.Alpha = 0;
	}

	public void Hide()
	{
		if (IsShown == false) return;
		IsShown = false;

		_sprite.Texture = Assets.GetTexture(DefaultTexturePath);
		_number.Alpha = 1;
	}
}
