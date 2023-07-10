using Azalea.Extentions;
using Azalea.Extentions.ObjectExtentions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Azalea.IO.Stores;

public class DllResourceStore : IResourceStore<byte[]>
{
    private readonly Assembly _assembly;

    private readonly string _prefix;

    public DllResourceStore(string dllName)
    {
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location).AsNotNull(), dllName);

        _assembly = File.Exists(filePath) ? Assembly.LoadFrom(filePath) : Assembly.Load(Path.GetFileNameWithoutExtension(dllName));

        _prefix = Path.GetFileNameWithoutExtension(dllName);
    }

    public DllResourceStore(Assembly assembly)
    {
        _assembly = assembly;
        _prefix = assembly.GetName().Name ??= "";
    }

    public DllResourceStore(AssemblyName name)
        : this(Assembly.Load(name)) { }

    public byte[]? Get(string name)
    {
        using Stream? input = GetStream(name);

        return input?.ReadAllBytesToArray();
    }

    public Stream? GetStream(string name)
    {
        string[] split = name.Split('/');

        for (int i = 0; i < split.Length - 1; i++)
            split[i] = split[i].Replace("-", "_");

        return _assembly?.GetManifestResourceStream($@"{_prefix}.{string.Join(".", split)}");
    }

    public IEnumerable<string> GetAvalibleResources() =>
        _assembly.GetManifestResourceNames().Select(n =>
        {
            n = n[(n.StartsWith(_prefix, StringComparison.Ordinal) ? _prefix.Length + 1 : 0)..];

            int lastDot = n.LastIndexOf('.');

            char[] chars = n.ToCharArray();

            for (int i = 0; i < lastDot; i++)
            {
                if (chars[i] == '.')
                    chars[i] = '/';
            }

            return new string(chars);
        });
}
