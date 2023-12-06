using System;
using System.Xml;

namespace Azalea.IO.Resources;
public static partial class ResourceExtentions_XML
{
	private static ResourceCache<XmlDocument> _xmlCache = new();
	public static XmlDocument? GetXML(this IResourceStore store, string path)
	{
		if (_xmlCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("Text could not be found.");

		var document = new XmlDocument();
		document.Load(stream);

		_xmlCache.AddValue(store, path, document);

		return document;
	}
}
