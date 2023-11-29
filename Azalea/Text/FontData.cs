using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using SharpFNT;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Azalea.Text;
public class FontData
{
	public string Name { get; init; }

	private IResourceStore _store;
	private string _path;
	private BitmapFont _font;

	public float Baseline => _font.Common.Base;

	public FontData(string name, IResourceStore store, string path)
	{
		Name = name;
		_store = store;
		_path = path;
		_font = store.GetBitmapFont(path) ?? throw new Exception("Font file not found");

		if (_path.EndsWith(".bin") || _path.EndsWith(".fnt")) _path = _path.Remove(_path.Length - 4);
	}

	public bool HasGlyph(char c) => _font.Characters.ContainsKey(c);
	public int GetKerning(char left, char right) => _font.GetKerningAmount(left, right);

	public TextureData GetPageImage(int page) => _store.GetTextureData(getFilenameForPage(page));
	private string getFilenameForPage(int page)
		=> $@"{_path}_{page.ToString().PadLeft((_font.Pages.Count - 1).ToString().Length, '0')}.png";

	private CharacterGlyph getCharacter(char character)
	{
		var bmCharacter = _font.GetCharacter(character);
		return new CharacterGlyph(character, bmCharacter.XOffset, bmCharacter.YOffset, bmCharacter.XAdvance, Baseline, this);
	}

	private Texture? getTexture(char character)
	{
		_font.Characters.TryGetValue(character, out Character? chr);
		if (chr is null) return null;

		var data = loadCharacter(chr);

		return Texture.FromData(AzaleaGame.Main.Host.Renderer, data);
	}

	private int _loadedGlyphCount;
	private TextureData loadCharacter(Character character)
	{
		var page = GetPageImage(character.Page);
		Debug.Assert(page != null);
		_loadedGlyphCount++;

		var source = page.Data;
		var target = new byte[character.Width * character.Height * 4];

		int readableHeight = Math.Min(character.Height, page.Height - character.Y);
		int readableWidth = Math.Min(character.Width, page.Width - character.X);

		for (int y = 0; y < character.Height; y++)
		{
			int readOffset = ((page.Width * (character.Y + y)) + character.X) * 4;
			int targetOffset = y * character.Width * 4;

			for (int x = 0; x < character.Width * 4; x += 4)
			{
				var sourcePixel = readOffset + x;
				var targetPixel = targetOffset + x;

				if (x / 4 < readableWidth && y / 4 < readableHeight)
				{
					target[targetPixel] = source[sourcePixel];
					target[targetPixel + 1] = source[sourcePixel + 1];
					target[targetPixel + 2] = source[sourcePixel + 2];
					target[targetPixel + 3] = source[sourcePixel + 3];
				}
				else
				{
					target[targetPixel] = byte.MaxValue;
					target[targetPixel + 1] = byte.MaxValue;
					target[targetPixel + 2] = byte.MaxValue;
					target[targetPixel + 3] = byte.MinValue;
				}
			}
		}

		var result = new ImageResult()
		{
			Width = character.Width,
			Height = character.Height,
			Data = target,
			Comp = ColorComponents.RedGreenBlueAlpha,
			SourceComp = ColorComponents.RedGreenBlueAlpha
		};
		return new TextureData(result);
	}

	private Dictionary<char, TexturedCharacterGlyph> _glyphCache = new();

	public TexturedCharacterGlyph? GetGlyph(char c)
	{
		if (_glyphCache.ContainsKey(c))
			return _glyphCache[c];


		var texture = getTexture(c);

		if (texture is null) return null;

		var glyph = new TexturedCharacterGlyph(getCharacter(c), texture, 1f / 100);
		_glyphCache[c] = glyph;

		return glyph;
	}
}
