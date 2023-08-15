using System;
using System.Collections.Generic;
using System.Linq;

namespace Azalea.IO.Stores;

public class NamespacedResourceStore<T> : ResourceStore<T>
    where T : class
{
    public string Namespace { get; set; }

    public NamespacedResourceStore(IResourceStore<T> store, string ns)
        : base(store)
    {
        Namespace = ns;
    }

    protected override IEnumerable<string> GetFilenames(string name) => base.GetFilenames($@"{Namespace}/{name}");

    public override IEnumerable<string> GetAvalibleResources()
        => base.GetAvalibleResources()
        .Where(x => x.StartsWith($"{Namespace}/", StringComparison.Ordinal))
        .Select(x => x[(Namespace.Length + 1)..]);
}
