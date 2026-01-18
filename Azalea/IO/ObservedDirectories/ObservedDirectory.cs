using Azalea.IO.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.ObservedDirectories;
public class ObservedDirectory
{
	List<FileSystemWatcher> watchers;
	public Dictionary<string, string> CurrentFiles = new Dictionary<string, string>();
	public Dictionary<string, string> CacheFiles = new Dictionary<string, string>();
	private string _cachePath;
	public ObservedDirectory(string[] paths, string cachePath, SerializeFileDelegate? serializeMethod)
	{
		_cachePath = cachePath;
		SerializeFileMethod = serializeMethod;

		var stream = Assets.PersistentStore.GetOrCreateStream(_cachePath);
		StreamReader reader = new StreamReader(stream);
		while (reader.Peek() != -1)
		{
			CacheFiles.Add(reader.ReadLine(), reader.ReadLine());
		}
		reader.Close();

		foreach (var (path, data) in CurrentFiles)
		{
			if (!CacheFiles.ContainsKey(path))
			{
				CacheFiles.Add(path, data);
			}
		}

		foreach (var (path, data) in CacheFiles)
		{
			if (!CurrentFiles.ContainsKey(path))
			{
				CacheFiles.Remove(path);
			}
		}

		watchers = new List<FileSystemWatcher>();
		foreach (string path in paths)
		{
			FileSystemWatcher watcher = new FileSystemWatcher(path);
			watcher.NotifyFilter = NotifyFilters.Attributes
										 | NotifyFilters.CreationTime
										 | NotifyFilters.DirectoryName
										 | NotifyFilters.FileName
										 | NotifyFilters.LastWrite
										 | NotifyFilters.Size;

			watcher.IncludeSubdirectories = true;

			watcher.Created += (ob, args) =>
			{
				Console.WriteLine($"Kreiran fajl u direktorijumu {args.Name}");
				if (SerializeFileMethod != null)
				{
					CurrentFiles.Add(args.FullPath, SerializeFileMethod(args.FullPath));
				}
				OnCreated?.Invoke(args.FullPath);
			};

			watcher.Changed += (ob, args) =>
			{
				if (Directory.Exists(args.FullPath))
					return;

				Console.WriteLine($"Promenjen fajl u direktorijumu {args.Name}");
				if (SerializeFileMethod != null)
				{
					CurrentFiles[args.FullPath] = SerializeFileMethod(args.FullPath);
				}
				OnModified?.Invoke(args.FullPath);
			};

			watcher.Deleted += (ob, args) =>
			{
				Console.WriteLine($"Obrisan fajl u direktorijumu {args.Name}");
				CurrentFiles.Remove(args.FullPath);
				OnDeleted?.Invoke(args.FullPath);
			};

			watcher.EnableRaisingEvents = true;
			watchers.Add(watcher);

		}

		foreach (string path in paths)
		{
			foreach (string file in GetItems(path))
			{
				CurrentFiles.Add(file, SerializeFileMethod is not null ? SerializeFileMethod(file) : "");
			}

		}

		foreach (KeyValuePair<string, string> file in CurrentFiles)
		{
			Console.WriteLine(file.Key);
			Console.WriteLine(file.Value);
		}


	}


	public void AddPath(string path) => throw new NotImplementedException();
	public void RemovePath(string path) => throw new NotImplementedException();

	List<string> _allPaths;
	/// <summary>
	/// Called when a file we have not seen before is created.
	/// </summary>
	public Action<string>? OnCreated;
	/// <summary>
	/// Called when a file we have seen before is read from the cache. Prvi string je path do tog fajla, bez timestamp. Drugi je data.
	/// </summary>
	public Action<string, string>? OnLoaded;
	/// <summary>
	/// Called when a file that is currently in memory is changed
	/// </summary>
	public Action<string>? OnModified;
	/// <summary>
	/// Called when a file currently in memory is deleted
	/// </summary>
	public Action<string>? OnDeleted;

	/// <summary>
	/// This method tells the <see cref="ObservedDirectory"/> how it
	/// should save files to the cache so it can load them later easier.
	/// If this is not set it will not attempt to save them.
	/// </summary>
	public SerializeFileDelegate? SerializeFileMethod;
	public delegate string SerializeFileDelegate(string path);

	public void SaveCache()
	{
		var stream = Assets.PersistentStore.GetOrCreateStream(_cachePath);
		StreamWriter writer = new StreamWriter(stream);
		foreach (var (path, data) in CurrentFiles)
		{
			writer.WriteLine(path);
			writer.WriteLine(data);
		}
		writer.Close();
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
					foreach (var file in GetItems(directory.FullName))
						yield return file;

				foreach (var file in directoryInfo.EnumerateFiles())
					yield return $"{file.FullName}|{file.LastWriteTime}";
			}
		}
	}
}
