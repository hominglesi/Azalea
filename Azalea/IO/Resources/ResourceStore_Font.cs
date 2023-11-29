using Azalea.Text;
using SharpFNT;
using System;
using System.IO;

namespace Azalea.IO.Resources;
public static class ResourceStore_Font
{
	private static ResourceCache<BitmapFont> _bitmapFontCache = new();
	public static BitmapFont? GetBitmapFont(this IResourceStore store, string path)
	{
		if (_bitmapFontCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("BitmapFont could not be found.");

		var bitmapFont = getBitmapFont(stream);
		_bitmapFontCache.AddValue(store, path, bitmapFont);

		return bitmapFont;
	}

	private static BitmapFont getBitmapFont(Stream stream)
	{
		return BitmapFont.FromStream(stream, FormatHint.Binary, false);
	}

	private static ResourceCache<FontData> _fontCache = new();
	public static TexturedCharacterGlyph? GetGlyph(this IResourceStore store, string fontName, char character)
	{
		var font = getFontByName(store, fontName) ?? throw new Exception($"No font by the name of {fontName} was loaded");

		return font.GetGlyph(character);
	}

	private static FontData? getFontByName(IResourceStore store, string name)
	{
		foreach (var item in _fontCache.GetAllStoreValues(store))
		{
			if (item.Name == name)
				return item;
		}

		return null;
	}

	public static void AddFont(this IResourceStore store, string path, string name)
	{
		_fontCache.AddValue(store, name, new FontData(name, store, path));
	}
}
