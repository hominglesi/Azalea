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
		if (File.Exists(_path + path) == false)
			return null;

		return File.OpenRead(_path + path);
	}

	public Stream GetOrCreateStream(string path)
	{
		return File.OpenWrite(_path + path);
	}

	public bool Exsists(string path)
	{
		return File.Exists(_path + path);
	}

	public IEnumerable<string> GetAvalibleResources()
	{
		throw new NotImplementedException();
	}
}
