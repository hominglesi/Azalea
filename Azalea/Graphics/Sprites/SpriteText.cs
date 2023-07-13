using Azalea.IO.Assets;
using Azalea.IO.Stores;
using Azalea.Text;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public partial class SpriteText : GameObject
{
    private static readonly char[] default_never_fixed_width_characters = { '.', ',', ':', ' ' };

    private FontStore _store { get; set; }

    public SpriteText()
    {
        _store = Assets.Fonts;
    }

    private string _text = "";

    public string Text
    {
        get => _text;
        set
        {
            if (_text.Equals(value))
                return;

            _text = value;
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

    private bool _useFullGlyphHeight = true;

    public bool UseFullGlyphHeight
    {
        get => _useFullGlyphHeight;
        set
        {
            if (_useFullGlyphHeight == value) return;

            _useFullGlyphHeight = value;

            invalidate(true, true);
        }
    }

    private bool _requiresAutoSizedWidth => _explicitWidth == null && (RelativeSizeAxes & Axes.X) == 0;
    private bool _requiresAutoSizedHeight => _explicitHeight == null && (RelativeSizeAxes & Axes.Y) == 0;

    private float? _explicitWidth;

    public override float Width
    {
        get
        {
            if (_requiresAutoSizedWidth)
                computeCharacters();
            return base.Width;
        }
        set
        {
            if (_explicitWidth == value)
                return;

            base.Width = value;
            _explicitWidth = value;

            invalidate(true, true);
        }
    }

    private float? _explicitHeight;

    public override float Height
    {
        get
        {
            if (_requiresAutoSizedHeight) computeCharacters();
            return base.Height;
        }
        set
        {
            if (_explicitHeight == value) return;

            base.Height = value;
            _explicitHeight = value;

            invalidate(true, true);
        }
    }

    private float _maxWidth = float.PositiveInfinity;

    public float MaxWidth
    {
        get => _maxWidth;
        set
        {
            if (_maxWidth == value) return;

            _maxWidth = value;
            invalidate(true, true);
        }
    }

    private MarginPadding _padding;

    public MarginPadding Padding
    {
        get => _padding;
        set
        {
            if (_padding.Equals(value)) return;

            _padding = value;

            invalidate(true, true);
        }
    }

    public override Vector2 Size
    {
        get
        {
            if (_requiresAutoSizedWidth || _requiresAutoSizedHeight)
                computeCharacters();

            return base.Size;
        }
        set
        {
            Width = value.X;
            Height = value.Y;
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

    public override bool IsPresent => base.IsPresent && (AlwaysPresent || string.IsNullOrEmpty(_displayedText) == false);

    protected override DrawNode CreateDrawNode() => new SpriteTextDrawNode(this);

    private void invalidate(bool chracter = false, bool textBuilder = false)
    {
        if (textBuilder)
            _textBuilderCache = null;
    }

    private string _displayedText => _text;


    private readonly List<TextBuilderGlyph> _characterBacking = new();

    private List<TextBuilderGlyph> _characters
    {
        get
        {
            computeCharacters();
            return _characterBacking;
        }
    }

    private void computeCharacters()
    {
        if (_store == null) return;

        _characterBacking.Clear();

        Vector2 textBounds = Vector2.Zero;

        try
        {
            if (string.IsNullOrEmpty(_displayedText))
                return;

            TextBuilder textBuilder = getTextBuilder();

            textBuilder.Reset();
            textBuilder.AddText(_displayedText);
            textBounds = textBuilder.Bounds;
        }
        finally
        {
            if (_requiresAutoSizedWidth)
                base.Width = textBounds.X + Padding.Right;
            if (_requiresAutoSizedHeight)
                base.Height = textBounds.Y + Padding.Bottom;

            base.Width = Math.Min(base.Width, MaxWidth);

            //_charactersCache.Validate();
        }
    }

    protected virtual char[]? FixedWidthExcludeCharacters => null;

    protected virtual char FixedWidthReferenceCharacter => 'm';

    protected virtual char FallbackCharacter => '?';

    private TextBuilder? _textBuilderCache;

    protected virtual TextBuilder CreateTextBuilder(ITexturedGlyphLookupStore store)
    {
        char[] excludeCharacters = FixedWidthExcludeCharacters ?? default_never_fixed_width_characters;

        float builderMaxWidth = _requiresAutoSizedWidth ? MaxWidth
            : ApplyRelativeAxes(RelativeSizeAxes, new Vector2(Math.Min(MaxWidth, base.Width), base.Height), FillMode).X - Padding.Right;

        return new TextBuilder(store, Font, builderMaxWidth, UseFullGlyphHeight, new Vector2(Padding.Left, Padding.Top), Spacing,
            _characterBacking, excludeCharacters, FallbackCharacter, FixedWidthReferenceCharacter);
    }

    private TextBuilder getTextBuilder()
    {
        if (_textBuilderCache is null)
            return _textBuilderCache = CreateTextBuilder(_store);

        return _textBuilderCache;
    }

    public float LineBaseHeight
    {
        get
        {
            computeCharacters();
            return _textBuilderCache == null ? (_textBuilderCache = getTextBuilder()).LineBaseHeight : _textBuilderCache.LineBaseHeight;
        }
    }
}
