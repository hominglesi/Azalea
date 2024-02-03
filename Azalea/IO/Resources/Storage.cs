using System;
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
		return File.OpenWrite(_path + path);
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

	public IEnumerable<string> GetAvalibleResources()
	{
		throw new NotImplementedException();
	}
}
