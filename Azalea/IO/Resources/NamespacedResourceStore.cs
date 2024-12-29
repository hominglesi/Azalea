using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Resources;
public class NamespacedResourceStore : IResourceStore
{
	private readonly IResourceStore _wrappedStore;
	private readonly string _namespace;

	public NamespacedResourceStore(IResourceStore wrappedStore, string name)
	{
		_wrappedStore = wrappedStore;
		_namespace = name;

		if (_namespace.EndsWith('/') == false)
			_namespace += '/';
	}

	public Stream? GetStream(string path)
	{
		return _wrappedStore.GetStream($"{_namespace}{path}");
	}

	public IEnumerable<(string, bool)> GetAvalibleResources(string subPath = "")
	{
		foreach (var resource in _wrappedStore.GetAvalibleResources(_namespace + subPath))
			yield return resource;
	}
}
