using System;

namespace Azalea.IO.ObservedDirectories;
public struct DirectoryFileData
{
	public string Path;
	public DateTime DateTime;

	public DirectoryFileData(string path, DateTime dateTime)
	{
		Path = path;
		DateTime = dateTime;
	}
}
