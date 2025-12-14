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
#if OLDTEXT

	public SpriteText()
	{
		AddLayout(_charactersCache);
	}

	private string _text = "";

	public string Text
	{
		get => _text;
		set
		{
			if (_text == value)
				return;

			_text = value;
			computeCharacters();
			invalidate(true, true);
		}
	}

	private FontUsage _font = FontUsage.Default;
	public FontUsage Font
	{
		get => _font;
		set
		{
			_font = value;
			invalidate(true, true);
		}
	}

	private Vector2 _spacing;

	public Vector2 Spacing
	{
		get => _spacing;
		set
		{
			if (_spacing == value) return;

			_spacing = value;
			invalidate(true, true);
		}
	}

	private Boundary _padding;

	public Boundary Padding
	{
		get => _padding;
		set
		{
			if (_padding.Equals(value)) return;
			_padding = value;

			invalidate(true, true);
		}
	}

	private List<ScreenSpaceCharacterPart>? _parts;

	public List<TextBuilderGlyph> Characters => _characters;
	private List<TextBuilderGlyph> _characters
	{
		get
		{
			computeCharacters();
			return _characterBacking;
		}
	}

	private bool _requiresAutoSizedWidth => (RelativeSizeAxes & Axes.X) == 0;
	private bool _requiresAutoSizedHeight => (RelativeSizeAxes & Axes.Y) == 0;

	private readonly List<TextBuilderGlyph> _characterBacking = new();
	private readonly LayoutValue _charactersCache = new(Invalidation.DrawSize | Invalidation.Presence, InvalidationSource.Parent);

	private void computeCharacters()
	{
		if (_charactersCache.IsValid) return;

		_characterBacking.Clear();

		Vector2 textBounds = Vector2.Zero;

		try
		{
			if (string.IsNullOrEmpty(Text))
				return;

			TextBuilder textBuilder = getTextBuilder();

			textBuilder.Reset();
			textBuilder.AddText(Text);
			textBounds = textBuilder.Bounds;
		}
		finally
		{
			if (_requiresAutoSizedWidth)
				Width = textBounds.X + Padding.Right;
			if (_requiresAutoSizedHeight)
				Height = textBounds.Y + Padding.Bottom;

			_charactersCache.Validate();
		}
	}

	private TextBuilder getTextBuilder()
	{
		if (_textBuilderCache is null)
			return _textBuilderCache = CreateTextBuilder();

		return _textBuilderCache;
	}

	private TextBuilder? _textBuilderCache;
	protected virtual char[]? FixedWidthExcludeCharacters => null;
	protected virtual char FixedWidthReferenceCharacter => 'm';
	protected virtual char FallbackCharacter => '?';
	private static readonly char[] default_never_fixed_width_characters = { '.', ',', ':', ' ' };
	protected virtual TextBuilder CreateTextBuilder()
	{
		char[] excludeCharacters = FixedWidthExcludeCharacters ?? default_never_fixed_width_characters;

		float builderMaxWidth = _requiresAutoSizedWidth ? float.PositiveInfinity
			: ApplyRelativeAxes(RelativeSizeAxes, new Vector2(Width, Height), FillMode).X - Padding.Right;

		return new TextBuilder(Font, builderMaxWidth, true, new Vector2(Padding.Left, Padding.Top), Spacing,
			_characterBacking, excludeCharacters, FallbackCharacter, FixedWidthReferenceCharacter);
	}

	public override void Draw(IRenderer renderer)
	{
		int partCount = Characters.Count;

		if (_parts == null)
			_parts = new List<ScreenSpaceCharacterPart>(partCount);
		else
		{
			_parts.Clear();
			_parts.EnsureCapacity(partCount);
		}

		foreach (var character in Characters)
		{
			_parts.Add(new ScreenSpaceCharacterPart
			{
				DrawQuad = ToScreenSpace(character.DrawRectangle),
				Texture = character.Texture
			});
		}

		Debug.Assert(_parts is not null);

		for (int i = 0; i < _parts.Count; i++)
		{
			renderer.DrawQuad(_parts[i].Texture, _parts[i].DrawQuad, DrawColorInfo);
		}
	}

	private void invalidate(bool characters = false, bool textBuilder = false)
	{
		if (characters)
			_charactersCache.Invalidate();

		if (textBuilder)
			_textBuilderCache = null;
	}

	internal struct ScreenSpaceCharacterPart
	{
		public Quad DrawQuad;

		public Texture Texture;
	}
#else
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
#endif
}
