using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Text;
internal class TextLayoutProvider()
{
	private bool _isValid = false;

	private MsdfFontData? _msdfData = Assets.MainStore.GetMsdfFontByName(FontUsage.DefaultFontName);

	private FontUsage _font = FontUsage.Default;
	public FontUsage Font
	{
		get => _font;
		set
		{
			if (_font == value)
				return;

			_isValid = false;
			_font = value;
			_msdfData = Assets.MainStore.GetMsdfFontByName(_font.FontName);
		}
	}

	private string _text = string.Empty;
	public string Text
	{
		get => _text;
		set
		{
			if (_text == value)
				return;

			_isValid = false;
			_text = value;
		}
	}

	public List<Character> _characters = [];
	public Vector2 _size;

	public List<Character> GetCharacters()
	{
		if (_isValid == false)
			computeCharacters();

		return _characters;
	}

	public Vector2 GetSize()
	{
		if (_isValid == false)
			computeCharacters();

		return _size;
	}

	private void computeCharacters()
	{
		var fontSize = _font.Size; // * 0.8f;

		float advance = 0, lineHeight = 0.85f;
		int i, j = 0;
		for (i = 0; i < _text.Length; i++)
		{
			var chr = _text[i];

			if (chr == ' ')
			{
				advance += _msdfData!.GetCharacter('j').HorizontalAdvance;
				continue;
			}

			var character = _characters.Count <= j ? new Character() : _characters[j];
			var msdfCharacter = _msdfData!.GetCharacter(chr);

			var position = msdfCharacter.EmPosition * fontSize;
			position.X += advance * fontSize;
			position.Y += lineHeight * fontSize;

			character.RepresentedCharacter = chr;
			character.Texture = msdfCharacter.Texture;
			character.DrawRectangle.Position = position;
			character.DrawRectangle.Size = msdfCharacter.EmSize * fontSize;

			advance += msdfCharacter.HorizontalAdvance;

			if (j < _characters.Count)
				_characters[j] = character;
			else
				_characters.Add(character);

			j++;
		}

		while (j < _characters.Count)
			_characters.RemoveAt(_characters.Count - 1);

		_size.X = advance * fontSize;
		_size.Y = fontSize;
		_isValid = true;
	}

	public struct Character
	{
		public char RepresentedCharacter;
		public Texture Texture;

		public Rectangle DrawRectangle;
	}
}
