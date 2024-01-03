using Azalea.Layout;
using Azalea.Text;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public partial class SpriteText : GameObject
{
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

	protected override DrawNode CreateDrawNode() => new SpriteTextDrawNode(this);

	private void invalidate(bool characters = false, bool textBuilder = false)
	{
		if (characters)
			_charactersCache.Invalidate();

		if (textBuilder)
			_textBuilderCache = null;
	}
}
