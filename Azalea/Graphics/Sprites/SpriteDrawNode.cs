using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using System;

namespace Azalea.Graphics.Sprites;

public class SpriteDrawNode : TexturedShaderDrawNode
{
	protected Texture? Texture { get; set; }
	protected float Time { get; set; }
	protected Quad ScreenSpaceDrawQuad { get; set; }

	protected new Sprite Source => (Sprite)base.Source;

	public SpriteDrawNode(IGameObject source)
		: base(source) { }

	public override void ApplyState()
	{
		base.ApplyState();

		Texture = Source.Texture;
		ScreenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
		Time = Source.Time;
	}

	protected virtual void Blit(IRenderer renderer)
	{
		if (Texture is null)
		{
			Console.WriteLine("Couldn't draw sprite because texture was null");
			return;
		}

		var texture = Texture;
		if (Texture is TextureAnimation anim)
		{
			texture = anim.GetTextureAtTime(Time);
		}

		renderer.DrawQuad(
			texture,
			ScreenSpaceDrawQuad,
			DrawColorInfo);
	}

	public override void Draw(IRenderer renderer)
	{
		base.Draw(renderer);

		Blit(renderer);
	}
}
