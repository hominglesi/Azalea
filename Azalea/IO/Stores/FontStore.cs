using Azalea.Extentions.ObjectExtentions;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Azalea.IO.Stores;

public class FontStore : TextureStore, ITexturedGlyphLookupStore
{
	private readonly List<IGlyphStore> _glyphStores = new();

	private readonly List<FontStore> _nestedFontStores = new();

	private readonly ConcurrentDictionary<(string, char), ITexturedCharacterGlyph?> _namespacedGlyphCache = new();

	public FontStore(IRenderer renderer, IResourceStore<TextureUpload>? store = null, float scaleAdjust = 100)
		: base(renderer, store, scaleAdjust)
	{

	}

	public override void AddTextureSource(IResourceStore<TextureUpload> store)
	{
		if (store is IGlyphStore gs)
		{
			_glyphStores.Add(gs);
		}

		base.AddTextureSource(store);
	}

	public override void AddStore(ITextureStore store)
	{
		if (store is FontStore fs)
		{
			_nestedFontStores.Add(fs);
		}

		base.AddStore(store);
	}

	public ITexturedCharacterGlyph? Get(string fontName, char character)
	{
		var key = (fontName, character);

		if (_namespacedGlyphCache.TryGetValue(key, out var exsisting))
			return exsisting;

		string textureName = string.IsNullOrEmpty(fontName) ? character.ToString() : $"{fontName}/{character}";

		foreach (var store in _glyphStores)
		{
			if ((string.IsNullOrEmpty(fontName) || fontName == store.FontName) && store.HasGlyph(character))
				return _namespacedGlyphCache[key] = new TexturedCharacterGlyph(store.Get(character).AsNotNull(), Get(textureName).AsNotNull(), 1 / ScaleAdjust);
		}

		foreach (var store in _nestedFontStores)
		{
			var glyph = store.Get(fontName, character);
			if (glyph != null)
				return glyph;
		}

		return _namespacedGlyphCache[key] = null;
	}
}
