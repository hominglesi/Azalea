using Azalea.Extentions;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Numerics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Text;

public class TextBuilder
{
	public Vector2 Bounds { get; private set; }

	public readonly List<TextBuilderGlyph> Characters;

	private readonly char[] _neverFixedWidthCharacters;
	private readonly char _fallbackCharacter;
	private readonly char _fixedWidthReferenceCharacter;
	private readonly FontUsage _font;
	private readonly bool _useFontSizeAsHeight;
	private readonly Vector2 _startOffset;
	private readonly Vector2 _spacing;
	private readonly float _maxWidth;

	private Vector2 _currentPosition;
	private float _currentLineHeight;
	private float? _currentLineBase;
	private bool _currentNewLine = true;

	public float LineBaseHeight
	{
		get
		{
			if (_currentPosition.Y > _startOffset.Y)
				throw new InvalidOperationException($"Cannot return a {nameof(LineBaseHeight)} from a text builder with multiple lines.");

			return _currentLineBase ?? 0;
		}
	}

	public TextBuilder(FontUsage font, float maxWidth = float.MaxValue, bool useFontSizeAsHeight = true,
		Vector2 startOffset = default, Vector2 spacing = default, List<TextBuilderGlyph>? characterList = null,
		char[]? neverFixedWidthCharacters = null, char fallbackCharacter = '?', char fixedWidthReferenceCharacter = 'm')
	{
		_font = font;
		_useFontSizeAsHeight = useFontSizeAsHeight;
		_startOffset = startOffset;
		_spacing = spacing;
		_maxWidth = maxWidth;

		Characters = characterList ?? new List<TextBuilderGlyph>();
		_neverFixedWidthCharacters = neverFixedWidthCharacters ?? Array.Empty<char>();
		_fallbackCharacter = fallbackCharacter;
		_fixedWidthReferenceCharacter = fixedWidthReferenceCharacter;

		_currentPosition = startOffset;
	}

	public virtual void Reset()
	{
		Bounds = Vector2.Zero;
		Characters.Clear();

		_currentPosition = _startOffset;
		_currentLineBase = null;
		_currentLineHeight = 0;
		_currentNewLine = true;
	}

	protected virtual bool CanAddCharacters => true;

	public void AddText(string text)
	{
		foreach (char c in text)
		{
			if (AddCharacter(c) == false)
				return;
		}
	}

	public bool AddCharacter(char character)
	{
		if (CanAddCharacters == false)
			return false;

		if (tryCreateGlyph(character, out var glyph) == false)
			return true;

		float kerning = 0;

		if (_currentNewLine == false)
		{
			if (Characters.Count > 0)
				kerning = glyph.GetKerning(Characters[^1].Glyph);
			kerning += _spacing.X;
		}

		if (HasAvalibleSpace(kerning + glyph.XAdvance) == false)
		{
			OnWidthExceeded();

			if (CanAddCharacters == false)
				return false;
		}

		_currentPosition.X += kerning;

		glyph.DrawRectangle = new Rectangle(
			new Vector2(_currentPosition.X + glyph.XOffset, _currentPosition.Y + glyph.YOffset),
			new Vector2(glyph.Width, glyph.Height));
		glyph.LinePosition = _currentPosition.Y;
		glyph.OnNewLine = _currentNewLine;

		if (glyph.IsWhiteSpace() == false)
		{
			if (glyph.Baseline > _currentLineBase)
			{
				for (int i = Characters.Count - 1; i >= 0; --i)
				{
					var previous = Characters[i];
					previous.DrawRectangle = previous.DrawRectangle.Offset(0, glyph.Baseline - _currentLineBase.Value);
					Characters[i] = previous;

					_currentLineHeight = Math.Max(_currentLineHeight, previous.DrawRectangle.Bottom - previous.LinePosition);

					if (previous.OnNewLine)
						break;
				}
			}
			else if (glyph.Baseline < _currentLineBase)
			{
				glyph.DrawRectangle = glyph.DrawRectangle.Offset(0, _currentLineBase.Value - glyph.Baseline);
				_currentLineHeight = Math.Max(_currentLineHeight, glyph.DrawRectangle.Bottom - glyph.LinePosition);
			}

			_currentLineHeight = Math.Max(_currentLineHeight, _useFontSizeAsHeight ? _font.Size : glyph.Height);
			_currentLineBase = _currentLineBase == null ? glyph.Baseline : Math.Max(_currentLineBase.Value, glyph.Baseline);
		}

		Characters.Add(glyph);

		_currentPosition.X += glyph.XAdvance;
		_currentNewLine = false;

		Bounds = Vector2Extentions.ComponentMax(Bounds, _currentPosition + new Vector2(0, _currentLineHeight));
		return true;
	}

	public void AddNewLine()
	{
		if (_currentNewLine)
			_currentLineHeight = _font.Size;

		_currentPosition.X = _startOffset.X;
		_currentPosition.Y = _currentLineHeight + _spacing.Y;

		_currentLineBase = null;
		_currentLineHeight = 0;
		_currentNewLine = true;
	}

	public void RemoveLastCharacter()
	{
		if (Characters.Count == 0)
			return;

		TextBuilderGlyph removedCharacter = Characters[^1];
		TextBuilderGlyph? previousCharacter = Characters.Count == 1 ? null : Characters[^2];

		Characters.RemoveAt(Characters.Count - 1);

		float? lastLineBase = _currentLineBase;

		_currentLineBase = null;
		_currentLineHeight = _useFontSizeAsHeight ? _font.Size : 0;

		for (int i = Characters.Count - 1; i >= 0; i--)
		{
			var character = Characters[i];

			if (character.IsWhiteSpace() == false)
			{
				_currentLineBase = _currentLineBase == null ? character.Baseline : Math.Max(_currentLineBase.Value, character.Baseline);
				_currentLineHeight = Math.Max(_currentLineHeight, character.DrawRectangle.Bottom - character.LinePosition);
			}

			if (character.OnNewLine)
				break;
		}

		if (removedCharacter.OnNewLine && previousCharacter != null)
		{
			_currentPosition.Y = previousCharacter.Value.LinePosition;

			_currentPosition.X = previousCharacter.Value.DrawRectangle.Left - previousCharacter.Value.XOffset + previousCharacter.Value.XAdvance;
		}
		else
		{
			_currentPosition.X -= removedCharacter.XAdvance;

			if (previousCharacter != null)
				_currentPosition.X -= removedCharacter.GetKerning(previousCharacter.Value) + _spacing.X;

			if (_currentLineBase < lastLineBase)
			{
				for (int i = Characters.Count - 1; i >= 0; i--)
				{
					var character = Characters[i];
					character.DrawRectangle = character.DrawRectangle.Offset(0, _currentLineBase.Value - lastLineBase.Value);
					Characters[i] = character;

					if (character.OnNewLine)
						break;
				}
			}
		}

		Bounds = Vector2.Zero;

		foreach (var character in Characters)
		{
			float characterRightBound = character.DrawRectangle.Left - character.XOffset + character.XAdvance;
			float characterBottomBound = _useFontSizeAsHeight ? character.LinePosition + _font.Size : character.DrawRectangle.Bottom;

			Bounds = Vector2Extentions.ComponentMax(Bounds, new Vector2(characterRightBound, characterBottomBound));
		}

		if (Characters.Count == 0)
			_currentNewLine = true;
	}

	protected virtual void OnWidthExceeded() { }

	protected virtual bool HasAvalibleSpace(float length) => _currentPosition.X + length <= _maxWidth;

	private float getConstantWidth() => getTexturedGlyph(_fixedWidthReferenceCharacter)?.Width ?? 0;

	private bool tryCreateGlyph(char character, out TextBuilderGlyph glyph)
	{
		var fontStoreGlyph = getTexturedGlyph(character);

		if (fontStoreGlyph == null)
		{
			glyph = default;
			return false;
		}

		glyph = new TextBuilderGlyph(fontStoreGlyph, _font.Size, useFontSizeAsHeight: _useFontSizeAsHeight);

		return true;
	}

	private ITexturedCharacterGlyph? getTexturedGlyph(char character)
	{
		return Assets.MainStore.GetGlyph(_font.FontName, character)
			?? Assets.MainStore.GetGlyph(string.Empty, character)
			?? Assets.MainStore.GetGlyph(_font.FontName, _fallbackCharacter)
			?? Assets.MainStore.GetGlyph(string.Empty, _fallbackCharacter);
	}
}
