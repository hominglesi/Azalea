using Azalea.Graphics.Rendering;
using Azalea.Graphics.Shaders;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using System;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public class Sprite : GameObject
{
	private Texture _texture = Assets.MissingTexture;
	public virtual Texture Texture
	{
		get => _texture;
		set
		{
			if (_texture == value) return;
			_texture = value;
			_time = 0;

			if (Size == Vector2.Zero)
				Size = new Vector2(_texture?.Width ?? 0, _texture?.Height ?? 0);
		}
	}

	internal IShader Shader;

	private Axes _flippedAxes = Axes.None;
	public Axes FlippedAxes
	{
		get => _flippedAxes;
		set => _flippedAxes = value;
	}

	public Sprite()
	{
		Shader = AzaleaGame.Main.Host.Renderer.QuadShader;
	}

	private float _time = 0;
	internal float Time => _time;

	protected override void Update()
	{
		base.Update();

		_time += Platform.Time.DeltaTime;
	}

	public override void Draw(IRenderer renderer)
	{
		if (Alpha <= 0) return;

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

		renderer.BindShader(Shader);

		renderer.DrawQuad(
			texture,
			ScreenSpaceDrawQuad,
			DrawColorInfo,
			_flippedAxes);
	}
}
