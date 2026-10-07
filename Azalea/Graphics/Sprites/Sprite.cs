using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.Coordination;
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

	public IShader? Shader { get; set; }

	private float _time = 0;
	internal float Time => _time;

	protected override void Update()
	{
		base.Update();

		_time += Platform.Time.DeltaTime;
	}

	private static IShader? _loadingShader;

	public override void Draw(RenderCoordinator coordinator)
	{
		if (Alpha <= 0) return;

		if (Texture is PromisedTexture promised && promised.IsLoaded == false)
		{
			_loadingShader ??= GameHost.Instance.Loader.LoadShader(
					Assets.GetText("Shaders/DefaultQuadVertex.glsl")!,
					Assets.GetText("Shaders/LoadingFragment.glsl")!);

			coordinator.BindShader(_loadingShader);

			coordinator.Uniform("u_Time", Time);
			coordinator.Uniform("u_Offset", ScreenSpaceDrawQuad.TopLeft.X, ScreenSpaceDrawQuad.TopLeft.Y);
			coordinator.Uniform("u_Resolution", ScreenSpaceDrawQuad.Width, ScreenSpaceDrawQuad.Height);
			coordinator.Uniform("u_ScreenResolution", App.Window.ClientSize.X, App.Window.ClientSize.Y);

			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Rectangle.One);
			coordinator.FlushRenderBatch();
			return;
		}


		if (Texture is not null)
			Texture.Bind(coordinator, Time);
		else
			coordinator.BindTexture(GameHost.Instance.Loader.WhitePixel);

		coordinator.BindShader(Shader ?? GameHost.Instance.Loader.DefaultQuadShader);

		if (Texture is null)
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Rectangle.One);
		else
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Texture.GetUVCoordinates(Time));
	}
}
