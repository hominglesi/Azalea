using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Azalea.IO.Resources;
public class EmbeddedResourceStore : IResourceStore
{
	private Assembly _assembly;
	private string _prefix;

	public EmbeddedResourceStore(Assembly assembly)
	{
		_assembly = assembly;
		_prefix = assembly.GetName().Name ??= "";

		//Assembly paths cannot have spaces in them and they are replaced with underscores
		_prefix = _prefix.Replace(' ', '_');
	}

	public Stream? GetStream(string path)
	{
		var resourcePath = getResourcePath(path);

		return _assembly?.GetManifestResourceStream(resourcePath);
	}

	private string getResourcePath(string path)
	{
		// We need to convert 
		//    Textures/Backgrounds/night-sky.png
		// to Assembly.Texture.Backgrounds.night-sky.png

		var split = path.Split('/');

		return $"{_prefix}.{string.Join('.', split)}";
	}

	public IEnumerable<(string, bool)> GetAvalibleResources(string subPath = "")
	{
		List<string> directories = new();

		foreach (var resourceName in _assembly.GetManifestResourceNames())
		{
			var resourcePath = convertResourcePath(resourceName);

			if (subPath.Length > 0 && resourcePath.StartsWith(subPath) == false)
				continue;

			resourcePath = resourcePath[subPath.Length..];

			var slashIndex = resourcePath.IndexOf('/');
			if (slashIndex != -1)
			{
				var directory = resourcePath[0..slashIndex];

				if (directories.Contains(directory))
					continue;

				directories.Add(directory);

				yield return (directory, true);
				continue;
			}

			yield return (resourcePath, false);
		}
	}

	private string convertResourcePath(string path)
	{
		if (path.StartsWith(_prefix))
			path = path[(_prefix.Length + 1)..];

		var lastDot = path.LastIndexOf('.');
		var chars = path.ToCharArray();

		for (int i = 0; i < lastDot; i++)
		{
			if (chars[i] == '.')
				chars[i] = '/';
		}

		return new string(chars);
	}
}
