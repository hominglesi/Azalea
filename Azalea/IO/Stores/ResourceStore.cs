using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azalea.IO.Stores;

public class ResourceStore<T> : IResourceStore<T>
	where T : class
{
	private readonly List<IResourceStore<T>> _stores = new();

	private readonly List<string> _searchExtentions = new();

	public ResourceStore(IResourceStore<T>? store = null)
	{
		if (store != null)
			AddStore(store);
	}

	public ResourceStore(IResourceStore<T>[] stores)
	{
		foreach (var store in stores)
			AddStore(store);
	}

	public virtual void AddStore(IResourceStore<T> store) => _stores.Add(store);

	public virtual void RemoveStore(IResourceStore<T> store) => _stores.Remove(store);

	public virtual T? Get(string name)
	{
		if (name is null)
			return null;

		var filenames = GetFilenames(name);

		foreach (var store in _stores)
		{
			foreach (var f in filenames)
			{
				T? result = store.Get(f);
				if (result is not null)
					return result;
			}
		}

		return null;
	}

	public Stream? GetStream(string name)
	{
		if (name is null)
			return null;

		var filenames = GetFilenames(name);

		foreach (var store in _stores)
		{
			foreach (var f in filenames)
			{
				Stream? result = store.GetStream(f);
				if (result is not null)
					return result;
			}
		}

		return null;
	}

	protected virtual IEnumerable<string> GetFilenames(string name)
	{
		yield return name;

		foreach (string ext in _searchExtentions)
			yield return $@"{name}.{ext}";
	}

	public void AddExtention(string extention)
	{
		extention = extention.Trim('.');

		if (_searchExtentions.Contains(extention) == false)
			_searchExtentions.Add(extention);
	}

	public virtual IEnumerable<string> GetAvalibleResources()
	{
		return _stores.SelectMany(s => s.GetAvalibleResources());
	}
}
