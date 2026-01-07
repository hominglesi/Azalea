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

	public IEnumerable<(string, bool)> GetAvalibleResources(string subPath = "")
	{
		throw new Exception("Cannot list all file system resources");
	}

	public IEnumerable<string> GetItems(string path = "")
	{
		if (path == "")
		{
			foreach (var drive in DriveInfo.GetDrives())
			{
				if (drive.IsReady)
					yield return drive.Name;
			}
		}
		else
		{
			var directoryInfo = new DirectoryInfo(path);
			bool readable = true;

			try
			{
				directoryInfo.EnumerateFiles();
			}
			catch (UnauthorizedAccessException)
			{
				readable = false;
			}

			if (readable)
			{
				foreach (var directory in directoryInfo.EnumerateDirectories())
					yield return $"{directory.FullName}\\";

				foreach (var file in directoryInfo.EnumerateFiles())
					yield return file.FullName;
			}
		}
	}
}
