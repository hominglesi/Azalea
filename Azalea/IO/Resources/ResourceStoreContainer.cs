using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Resources;
public class ResourceStoreContainer : IResourceStore
{
	private readonly List<IResourceStore> _stores = new();

	public void AddStore(IResourceStore store)
	{
		_stores.Add(store);
	}

	public Stream? GetStream(string path)
	{
		for (int i = 0; i < _stores.Count; i++)
		{
			var stream = _stores[i].GetStream(path);
			if (stream != null)
				return stream;
		}

		return null;
	}

	public IEnumerable<(string, bool)> GetAvalibleResources(string subPath = "")
	{
		foreach (var store in _stores)
			foreach (var resource in store.GetAvalibleResources(subPath))
				yield return resource;
	}
}
