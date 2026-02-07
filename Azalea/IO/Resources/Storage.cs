using Azalea.Utils;
using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Resources;
public class Storage : IResourceStore
{
	public string Path { get; init; }

	public Storage(string path)
	{
		Path = path;

		if (Path.EndsWith('/') == false)
			Path += '/';
	}

	public Stream? GetStream(string path)
	{
		if (Exists(path) == false)
			return null;

		return File.OpenRead(Path + path);
	}

	public Stream GetOrCreateStream(string path)
	{
		var fullPath = Path + path;
		var directoryPath = System.IO.Path.GetDirectoryName(fullPath)!;

		if (Directory.Exists(directoryPath) == false)
			Directory.CreateDirectory(directoryPath);

		return new FileStream(fullPath, FileMode.OpenOrCreate);
	}

	public bool Exists(string path)
	{
		return File.Exists(Path + path);
	}

	public void Delete(string path)
	{
		var fullPath = Path + path;

		if (Directory.Exists(fullPath))
			Directory.Delete(fullPath, true);

		else if (Exists(path))
			File.Delete(fullPath);
	}

	private readonly static char[] _directorySeperators = ['/', '\\'];

	public IEnumerable<(string, bool)> GetAvalibleResources(string subPath = "")
	{
		var path = PathUtils.CombinePaths(Path, subPath);

		var files = Directory.GetFiles(path);
		var directories = Directory.GetDirectories(path);

		foreach (var directory in directories)
		{
			var lastSlash = directory.LastIndexOfAny(_directorySeperators) + 1;
			yield return (directory[lastSlash..], true);
		}

		foreach (var file in files)
			yield return (System.IO.Path.GetFileName(file), false);
	}
}
