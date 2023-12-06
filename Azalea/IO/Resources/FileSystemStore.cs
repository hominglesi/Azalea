using System;
using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Resources;
public class FileSystemStore : IResourceStore
{
	public Stream? GetStream(string path)
	{
		return File.OpenRead(path);
	}

	public IEnumerable<string> GetAvalibleResources()
	{
		throw new Exception("Cannot list all file system resources");
	}
}
