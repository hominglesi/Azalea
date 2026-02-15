using Azalea.Graphics.Rendering;
using Azalea.Graphics.Shaders;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Platform;
using System;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public class Sprite : GameObject
{
	private ITexture _texture = Assets.MissingTexture;
	public virtual ITexture Texture
	{
		get => _texture;
		set
		{
			//Edge case where size would stay 0 even though we set a valid texture
			if (value == Assets.MissingTexture && Size == Vector2.Zero)
				Size = Assets.MissingTexture.Size;

			if (_texture == value) return;
			_texture = value;
			_time = 0;

			if (Size == Vector2.Zero)
				Size = new Vector2(_texture?.Width ?? 0, _texture?.Height ?? 0);
		}
	}

	private float _time = 0;
	internal float Time => _time;

	protected override void Update()
	{
		base.Update();

		_time += Platform.Time.DeltaTime;
	}

	private static Shader? _loadingShader = null;

	public override void Draw(IRenderer renderer)
	{
		if (Alpha <= 0) return;

		if (Texture is null)
		{
			Console.WriteLine("Couldn't draw sprite because texture was null");
			return;
		}

		if (Texture is PromisedTexture promised && promised.IsResolved == false)
		{
			_loadingShader ??= Assets.MainStore.GetShader("Shaders/quad_vertex.glsl", "Shaders/loading_fragment.glsl");

			renderer.BindShader(_loadingShader);

			_loadingShader.NativeShader.SetUniform("u_Time", Time);
			_loadingShader.NativeShader.SetUniform("u_Offset", ScreenSpaceDrawQuad.TopLeft.X, ScreenSpaceDrawQuad.TopLeft.Y);
			_loadingShader.NativeShader.SetUniform("u_Resolution", ScreenSpaceDrawQuad.Width, ScreenSpaceDrawQuad.Height);
			_loadingShader.NativeShader.SetUniform("u_ScreenResolution", Window.ClientSize.X, Window.ClientSize.Y);

			renderer.DrawQuad(renderer.WhitePixel.GetNativeTexture(), ScreenSpaceDrawQuad, DrawColorInfo);

			renderer.BindShader(renderer.DefaultQuadShader);
			return;
		}

		DrawTexture(renderer, Texture);
	}

	protected virtual void DrawTexture(IRenderer renderer, ITexture texture)
		=> renderer.DrawQuad(texture.GetNativeTexture(Time), ScreenSpaceDrawQuad, DrawColorInfo, texture.GetUVCoordinates(Time));
}
