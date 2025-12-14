using Azalea.Graphics.Textures;
using Azalea.Numerics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Azalea.Text.Msdf;
public class MsdfFontData
{
	private Texture _spriteSheet;

	private Dictionary<char, Character> _characters;

	public MsdfFontData(string csvData, Texture spriteSheet)
	{
		_spriteSheet = spriteSheet;

		var csvSpan = csvData.AsSpan();
		var csvLineCount = Regex.Matches(csvData, System.Environment.NewLine).Count;

		_characters = new Dictionary<char, Character>(csvLineCount);

		Span<Range> csvLines = stackalloc Range[csvLineCount];
		Span<Range> csvLineArgs = stackalloc Range[10];

		csvSpan.Split(csvLines, Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

		foreach (var range in csvLines)
		{
			var line = csvSpan[range];

			line.Split(csvLineArgs, ',');

			var charCode = (char)int.Parse(line[csvLineArgs[0]]);
			var horizontalAdvance = float.Parse(line[csvLineArgs[1]]);

			var emX = float.Parse(line[csvLineArgs[2]]);
			var emY = float.Parse(line[csvLineArgs[3]]);
			var emWidth = float.Parse(line[csvLineArgs[4]]) - emX;
			var emHeigth = float.Parse(line[csvLineArgs[5]]) - emY;

			var x = (int)Math.Ceiling(float.Parse(line[csvLineArgs[6]]));
			var y = (int)Math.Ceiling(float.Parse(line[csvLineArgs[7]]));
			var width = (int)Math.Floor(float.Parse(line[csvLineArgs[8]]) - x);
			var height = (int)Math.Floor(float.Parse(line[csvLineArgs[9]]) - y);

			var character = new Character(charCode, horizontalAdvance,
				new Vector2(emX, emY), new Vector2(emWidth, emHeigth),
				new Vector2Int(x, y), new Vector2Int(width, height));

			_characters[charCode] = character;
		}
	}

	public Character GetCharacter(char charCode)
	{
		if (_characters.TryGetValue(charCode, out Character character) == false)
			character = _characters['?'];

		// We lazily create the texture to avoid having many texture regions that might
		// never be used
		if (character._texture is null)
		{
			character._texture = new TextureRegion(_spriteSheet,
				new RectangleInt(character.Position, character.Size));

			_characters[charCode] = character;
		}

		return character;
	}

	public struct Character(char charCode, float horizontalAdvance,
		Vector2 emPosition, Vector2 emSize, Vector2Int position, Vector2Int size)
	{
		public readonly char CharCode = charCode;

		public readonly float HorizontalAdvance = horizontalAdvance;

		public readonly Vector2 EmPosition = emPosition;
		public readonly Vector2 EmSize = emSize;

		public readonly Vector2Int Position = position;
		public readonly Vector2Int Size = size;

		internal TextureRegion? _texture;
		public readonly TextureRegion Texture => _texture!;
	}
}
