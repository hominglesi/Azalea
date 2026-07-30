using Azalea.Graphics.Rendering;
using Azalea.Graphics.Shaders;
using Azalea.IO.Resources;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Text;
using System;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public class SpriteText : GameObject
{
	private static Shader _textShader;

	static SpriteText()
	{
		_textShader = ShaderBuilder.FromShaderCode(
			Assets.GetText("Shaders/quad_vertex.glsl")!,
			Assets.GetText("Shaders/text_fragment.glsl")!);

		ShaderLibrary.RegisterShader("TextShader", _textShader);
	}

	private TextLayoutProvider _layoutProvider = new();

	public FontUsage Font
	{
		get => _layoutProvider.Font;
		set
		{
			if (_layoutProvider.Font == value)
				return;

			_layoutProvider.Font = value;

			if (string.IsNullOrEmpty(Text) == false)
				base.Size = _layoutProvider.GetSize();
		}
	}

	public string Text
	{
		get => _layoutProvider.Text;
		set
		{
			if (_layoutProvider.Text == value)
				return;

			_layoutProvider.Text = value;
			base.Size = _layoutProvider.GetSize();
		}
	}

	public new float Width
	{
		get => base.Width;
		set => throw new InvalidOperationException($"Cannot set {nameof(Width)} of {nameof(SpriteText)}");
	}

	public new float Height
	{
		get => base.Height;
		set => throw new InvalidOperationException($"Cannot set {nameof(Height)} of {nameof(SpriteText)}");
	}

	public new Vector2 Size
	{
		get => base.Size;
		set => throw new InvalidOperationException($"Cannot set {nameof(Size)} of {nameof(SpriteText)}");
	}

	public override void Draw(IRenderer renderer, RenderCoordinator? coordinator)
	{
		if (coordinator is not null)
			coordinator.BindProgram(_textShader.NewProgram!);
		else
			renderer?.BindShader(_textShader);

		foreach (var character in _layoutProvider.GetCharacters())
		{
			var quad = ToScreenSpace(character.DrawRectangle);

			if (coordinator is not null)
			{
				if (character.Texture.NewTexture is not null)
					coordinator.BindTexture(character.Texture.NewTexture!);

				coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, quad, DrawColorInfo.Color, character.Texture.GetUVCoordinates());
			}
			else
				renderer?.DrawQuad(character.Texture.GetNativeTexture(), quad, DrawColorInfo, character.Texture.GetUVCoordinates());
		}

		renderer?.BindShader(renderer.DefaultQuadShader);
	}
}
