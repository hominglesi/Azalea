using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using System;

namespace Azalea.Scenes.Drawables2D;

public class Box : Sprite
{
	public Box()
	{
		base.Texture = Assets.WhitePixel;
	}

	public override ITexture? Texture
	{
		get => base.Texture;
		set => throw new InvalidOperationException($"The texture of a {nameof(Box)} cannot be set");
	}
}
