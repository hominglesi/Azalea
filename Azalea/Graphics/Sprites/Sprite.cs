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

		switch (Texture)
		{
			case Texture tex:
				if (tex.IsLoaded)
				{
					coordinator.BindShader(Shader ?? GameHost.Instance.Loader.DefaultQuadShader);
					coordinator.BindTexture(tex.NativeTexture);
					coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, tex.UVCoordinates);
				}
				else
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
				coordinator.BindTexture(Assets.WhitePixelNative);
				coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Rectangle.One);
				break;
		}
	}
}
