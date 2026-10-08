using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using System;

namespace Azalea.Design.Shapes;

public class Box : Sprite
{
	public Box()
	{
		base.Texture = Assets.WhitePixel;
	}

	public override ITexture Texture
	{
		get => base.Texture;
		set => throw new InvalidOperationException($"The texture of a {nameof(Box)} cannot be set");
	}
}
