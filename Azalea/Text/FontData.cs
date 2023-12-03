using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using SharpFNT;
using System;
using System.Collections.Generic;

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
	public Texture GetPageImage(int page) => _store.GetTexture(getFilenameForPage(page));

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

		return new TextureRegion(GetPageImage(chr.Page), new(chr.X, chr.Y, chr.Width, chr.Height));
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
