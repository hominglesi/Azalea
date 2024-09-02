using Azalea.Utils;
using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Resources;
public class Storage : IResourceStore
{
	private string _path;

	public Storage(string path)
	{
		_path = path;

		if (_path.EndsWith('/') == false)
			_path += '/';
	}

	public Stream? GetStream(string path)
	{
		if (Exists(path) == false)
			return null;

		return File.OpenRead(_path + path);
	}

	public Stream GetOrCreateStream(string path)
	{
		var fullPath = _path + path;
		var directoryPath = Path.GetDirectoryName(fullPath)!;

		if (Directory.Exists(directoryPath) == false)
			Directory.CreateDirectory(directoryPath);

		return new FileStream(fullPath, FileMode.OpenOrCreate);
	}

	public bool Exists(string path)
	{
		return File.Exists(_path + path);
	}

	public void Delete(string path)
	{
		if (Exists(path))
			File.Delete(_path + path);
	}

	public IEnumerable<string> GetContainedResources(string subPath = "")
	{
		var path = FileSystemUtils.CombinePaths(_path, subPath);

		var files = Directory.GetFiles(path);
		var directories = Directory.GetDirectories(path);

		foreach (var directory in directories)
			yield return directory;

		foreach (var file in files)
			yield return file;
	}

	public IEnumerable<string> GetAvalibleResources()
	{
		return GetContainedResources();
	}
}
