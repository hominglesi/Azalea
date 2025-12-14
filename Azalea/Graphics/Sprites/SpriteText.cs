using Azalea.Graphics.Rendering;
using Azalea.Text;
using System;
using System.Numerics;
using Azalea.Inputs;
using Azalea.Editing;

#if OLDTEXT
using System.Numerics;
using System.Collections.Generic;
using Azalea.Layout;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Textures;
using System.Diagnostics;
#else
using Azalea.Graphics.Shaders;
using Azalea.IO.Resources;
#endif

namespace Azalea.Graphics.Sprites;

public class SpriteText : GameObject
{
	private static Shader _textShader;

	static SpriteText()
	{
		_textShader = Assets.MainStore.GetShader("Shaders/text_vertex.glsl", "Shaders/text_fragment.glsl");
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

	protected override void Update()
	{
		if (Input.GetKey(Keys.J).Down)
			Editor.HighlightObject(this);
	}

	public override void Draw(IRenderer renderer)
	{
		renderer.BindShader(_textShader);

		foreach (var character in _layoutProvider.GetCharacters())
		{
			var quad = ToScreenSpace(character.DrawRectangle);

			renderer.DrawQuad(character.Texture, quad, DrawColorInfo);
		}

		renderer.BindShader(renderer.DefaultQuadShader);
	}
}
