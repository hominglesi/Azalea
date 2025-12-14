using Azalea.Graphics.Textures;
using Azalea.Text;
using System;

namespace Azalea.IO.Resources;

public static partial class ResourceStoreExtentions
{
	private static ResourceCache<MsdfFontData> _msdfFontDataCache = new();

	public static MsdfFontData GetMsdfFont(this IResourceStore store, string csvPath, string spriteSheetPath)
	{
		var combinedPath = $"{csvPath}+{spriteSheetPath}";

		if (_msdfFontDataCache.TryGetValue(store, combinedPath, out var cached))
			return cached;

		var csvData = store.GetText(csvPath)
			?? throw new Exception("Font csv data could not be found.");

		var spriteSheet = store.GetTexture(spriteSheetPath, TextureFiltering.Linear)
			?? throw new Exception("Font sprite sheet could not be found.");

		var msdfData = new MsdfFontData(csvData, spriteSheet);
		_msdfFontDataCache.AddValue(store, combinedPath, msdfData);

		return msdfData;
	}

	private static ResourceCache<(string, string)> _msdfFontNameCache = new();

	public static MsdfFontData GetMsdfFontByName(this IResourceStore store, string name)
	{
		if (_msdfFontNameCache.TryGetValue(store, name, out var paths) == false)
			throw new Exception($"The font {name} has not been loaded.");

		return GetMsdfFont(store, paths.Item1, paths.Item2);
	}

	public static void AddMsdfFont(this IResourceStore store, string name,
		string csvPath, string spriteSheetPath)
		=> _msdfFontNameCache.AddValue(store, name, (csvPath, spriteSheetPath));
}
