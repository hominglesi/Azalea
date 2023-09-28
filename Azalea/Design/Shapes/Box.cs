using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using System;

namespace Azalea.Design.Shapes;

public class Box : Sprite
{
	public Box()
	{
		base.Texture = AzaleaGame.Main.Host.Renderer.WhitePixel;
	}

	public override Texture? Texture
	{
		get => base.Texture;
		set => throw new InvalidOperationException($"The texture of a {nameof(Box)} cannot be set");
	}
}
