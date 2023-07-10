using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azalea.IO.Stores;

public class ResourceStore<T> : IResourceStore<T>
    where T : class
{
    private readonly List<IResourceStore<T>> _stores = new List<IResourceStore<T>>();

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

        foreach (var store in _stores)
        {
            T? result = store.Get(name);
            if (result is not null)
                return result;
        }

        return null;
    }

    public Stream? GetStream(string name)
    {
        if (name is null)
            return null;

        foreach (var store in _stores)
        {
            var result = store.GetStream(name);
            if (result is not null)
                return result;
        }

        return null;
    }

    public virtual IEnumerable<string> GetAvalibleResources()
    {
        return _stores.SelectMany(s => s.GetAvalibleResources());
    }
}
