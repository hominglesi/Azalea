using Azalea.Graphics.Shaders;
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
			{
				switch (_texture)
				{
					case Texture tex:
						Size = new Vector2(tex.Width, tex.Height);
						break;
					case TextureAnimation texAnim:
						if (texAnim.Frames.Count == 0)
							break;

						var first = texAnim.GetTextureAt(0);
						Size = first.Size;
						break;
				}
			}
		}
	}
	public Texture? FallbackTexture { get; set; }

	public Shader? Shader { get; set; }

	private static LoadingShader? _loadingShader;
	public static LoadingShader LoadingShader
	{
		get
		{
			if (_loadingShader is not null)
				return _loadingShader;

			_loadingShader = new LoadingShader();

			return _loadingShader;
		}
	}

	private float _time = 0;
	internal float Time => _time;

	protected override void Update()
	{
		base.Update();

		_time += App.Scheduler.DeltaTime;
	}

	public override void Draw(RenderCoordinator coordinator)
	{
		if (Alpha <= 0) return;

		switch (Texture)
		{
			case Texture tex:
				switch (tex.NativeTexture.State)
				{
					case TextureState.Ready:
						coordinator.BindShader(Shader ?? GameHost.Instance.Loader.DefaultQuadShader);
						coordinator.BindTexture(tex.NativeTexture);
						coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, tex.UVCoordinates);
						break;
					case TextureState.Loading:
						coordinator.BindShader(LoadingShader);
						LoadingShader.Uniform(coordinator.CommandQueue, "u_Time", Time);
						LoadingShader.Uniform(coordinator.CommandQueue, "u_Offset", ScreenSpaceDrawQuad.TopLeft.X, ScreenSpaceDrawQuad.TopLeft.Y);
						LoadingShader.Uniform(coordinator.CommandQueue, "u_Resolution", ScreenSpaceDrawQuad.Width, ScreenSpaceDrawQuad.Height);
						LoadingShader.Uniform(coordinator.CommandQueue, "u_ScreenResolution", App.Window.ClientSize.X, App.Window.ClientSize.Y);

						LoadingShader.Uniform(coordinator.CommandQueue, "u_BackgroundColor", LoadingShader.BackgroundColor);
						LoadingShader.Uniform(coordinator.CommandQueue, "u_RingColor", LoadingShader.RingColor);
						LoadingShader.Uniform(coordinator.CommandQueue, "u_CircleColor", LoadingShader.CircleColor);

						coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Rectangle.One);
						coordinator.FlushRenderBatch();
						break;
					case TextureState.Missing:
						coordinator.BindShader(Shader ?? GameHost.Instance.Loader.DefaultQuadShader);
						coordinator.BindTexture(FallbackTexture?.NativeTexture ?? Assets.MissingTexture.NativeTexture);
						coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Rectangle.One);
						break;
				}

				break;
			case TextureAnimation texAnim:
				coordinator.BindShader(Shader ?? GameHost.Instance.Loader.DefaultQuadShader);
				var texAnimFrame = texAnim.GetTextureAt(Time);
				coordinator.BindTexture(texAnimFrame.NativeTexture);
				coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, texAnimFrame.UVCoordinates);
				break;
			default:
				coordinator.BindShader(Shader ?? GameHost.Instance.Loader.DefaultQuadShader);
				coordinator.BindTexture(FallbackTexture?.NativeTexture ?? Assets.WhitePixelNative);
				coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Rectangle.One);
				break;
		}
	}
}
