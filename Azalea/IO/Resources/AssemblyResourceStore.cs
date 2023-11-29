using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Azalea.IO.Resources;
public class AssemblyResourceStore : IResourceStore
{
	private Assembly _assembly;
	private string _prefix;

	public AssemblyResourceStore(Assembly assembly)
	{
		_assembly = assembly;
		_prefix = assembly.GetName().Name ??= "";

		//Assembly paths cannot have spaces in them and they are replaced with underscores
		_prefix = _prefix.Replace(' ', '_');
	}

	public Stream? GetStream(string path)
	{
		var resourcePath = getResourcePath(path);

		var x = _assembly.GetManifestResourceNames();

		return _assembly?.GetManifestResourceStream(resourcePath);
	}

	private string getResourcePath(string path)
	{
		//We need to convert 
		//     Textures/Backgrounds/night-sky.png
		//to   Assembly.Texture.Backgrounds.night-sky.png

		var split = path.Split('/');

		return $"{_prefix}.{string.Join('.', split)}";
	}

	public IEnumerable<string> GetAvalibleResources()
	{
		foreach (var item in _assembly.GetManifestResourceNames())
			yield return getDirectoryPath(item);
	}

	private string getDirectoryPath(string path)
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
