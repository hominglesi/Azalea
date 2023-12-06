using System;
using System.IO;

namespace Azalea.IO.Resources;
public static partial class ResourceStoreExtentions
{
	private static ResourceCache<string> _textCache = new();
	public static string? GetText(this IResourceStore store, string path)
	{
		if (_textCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("Text could not be found.");

		using var reader = new StreamReader(stream);
		var text = reader.ReadToEnd();
		_textCache.AddValue(store, path, text);

		return text;
	}
}
