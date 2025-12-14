using System.Collections.Generic;

namespace Azalea.IO.Resources;
public class ResourceCache<T>
{
	private static Dictionary<IResourceStore, Dictionary<string, T>> _cache = new();
	public bool TryGetValue(IResourceStore store, string path, out T value)
	{
		value = default!;
		if (_cache.ContainsKey(store) == false)
			return false;

		var cache = _cache[store];
		if (cache.ContainsKey(path))
		{
			value = cache[path];
			return true;
		}

		return false;
	}

	public void AddValue(IResourceStore store, string path, T resource)
	{
		if (_cache.ContainsKey(store) == false)
			_cache[store] = new Dictionary<string, T>();

		_cache[store][path] = resource;
	}

	public IEnumerable<T> GetAllStoreValues(IResourceStore store)
	{
		var cache = _cache[store];
		foreach (var value in cache.Values)
			yield return value;
	}
}
