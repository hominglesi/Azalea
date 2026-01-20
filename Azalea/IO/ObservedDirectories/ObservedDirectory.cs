using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azalea.IO.ObservedDirectories;
public class ObservedDirectory : Disposable
{
	List<FileSystemWatcher> watchers;
	public Dictionary<DirectoryFileData, string> CurrentFiles = new Dictionary<DirectoryFileData, string>();
	public Dictionary<DirectoryFileData, string> CacheFiles = new Dictionary<DirectoryFileData, string>();
	private string _cachePath;
	private bool _isRunning = false;
	List<string> _allPaths = new List<string>();
	public ObservedDirectory(string[] paths, string cachePath, SerializeFileDelegate? serializeMethod)
	{
		_cachePath = cachePath;
		SerializeFileMethod = serializeMethod;
		_allPaths.AddRange(paths);


	}

	public void Start()
	{
		if (_isRunning)
			return;

		_isRunning = true;

		watchers = new List<FileSystemWatcher>();

		foreach (var path in _allPaths)
			if (Directory.Exists(path) == false)
				throw new Exception("The provided directory does not exists");

		var stream = Assets.PersistentStore.GetOrCreateStream(_cachePath);
		StreamReader reader = new StreamReader(stream);
		while (reader.Peek() != -1)
		{

			string metaData = reader.ReadLine();
			string path = metaData.Split('|')[0];
			DateTime dateTime = DateTime.Parse(metaData.Split("|")[1]);
			string data = reader.ReadLine();
			CacheFiles.Add(new(path, dateTime), data);
			OnLoaded?.Invoke(path, data);
		}
		reader.Close();

		ProcessPaths(_allPaths);


	}


	private void ProcessPaths(IEnumerable<string> paths)
	{


		foreach (string path in paths)
		{
			if (!_allPaths.Contains(path))
				_allPaths.Add(path);

			foreach (string file in GetItems(path))
			{

				CurrentFiles.Add(GetMetaDataFromPath(file), SerializeFileMethod is not null ? SerializeFileMethod(GetPathFromFullPath(file)) : "");
			}

		}

		UpdateCacheInMemory();

		foreach (string path in paths)
		{
			CreateWatcherOnPath(path);
		}
	}

	private void UpdateCacheInMemory()
	{
		foreach (var (metaData, data) in CurrentFiles)
		{
			if (!CacheFiles.ContainsKey(metaData))
			{
				OnCreated?.Invoke(metaData.Path);
				CacheFiles.Add(metaData, data);
			}
		}

		foreach (var (metaData, data) in CacheFiles)
		{
			if (!CurrentFiles.ContainsKey(metaData))
			{
				OnDeleted?.Invoke(metaData.Path);
				CacheFiles.Remove(metaData);
			}
		}
	}
	private void CreateWatcherOnPath(string path)
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

			DirectoryFileData metaData;
			metaData.Path = args.FullPath;
			metaData.DateTime = File.GetLastWriteTime(args.FullPath);
			if (SerializeFileMethod != null)
			{
				CurrentFiles.Add(metaData, SerializeFileMethod(GetPathFromFullPath(metaData.Path)));
				Console.WriteLine($"Kreiran fajl u direktorijumu {args.Name}");
			}
			OnCreated?.Invoke(metaData.Path);
		};

		watcher.Changed += (ob, args) =>
		{

			if (Directory.Exists(args.FullPath))
				return;

			DirectoryFileData metaData;
			metaData.Path = args.FullPath;
			metaData.DateTime = File.GetLastWriteTime(args.FullPath);

			if (SerializeFileMethod != null)
			{
				CurrentFiles[metaData] = SerializeFileMethod(GetPathFromFullPath(args.FullPath));
				Console.WriteLine($"Promenjen fajl u direktorijumu {args.Name}");
			}
			OnModified?.Invoke(metaData.Path);
		};

		watcher.Deleted += (ob, args) =>
		{
			DirectoryFileData metaData;
			metaData.Path = args.FullPath;
			metaData.DateTime = File.GetLastWriteTime(args.FullPath);

			CurrentFiles.Remove(metaData);
			Console.WriteLine($"Obrisan fajl u direktorijumu {args.Name}");

			OnDeleted?.Invoke(metaData.Path);
		};

		watcher.EnableRaisingEvents = true;
		watchers.Add(watcher);
	}

	private string GetPathFromFullPath(string fullPath)
	{
		string gas = fullPath.Split('|')[0];
		return gas;

	}

	private DirectoryFileData GetMetaDataFromPath(string fullPath)
	{
		return new(fullPath.Split('|')[0], DateTime.Parse(fullPath.Split('|')[1]));
	}

	protected override void OnDispose()
	{
		if (watchers == null)
			return;
		foreach (var watcher in watchers)
		{
			watcher.Dispose();
		}
		watchers.Clear();
	}
	public void AddPath(string path)
	{

		ProcessPaths([path]);
	}
	public void RemovePath(string path)
	{
		_allPaths.Remove(path);
		Console.WriteLine("Test: " + path);
		foreach (var file in CurrentFiles)
		{
			Console.WriteLine(file.Key.Path);
		}

		var keys = CurrentFiles
			.Where(x => x.Key.Path.Contains(path.Replace('/', '\\')))
			.Select(x => x.Key)
			.ToList();

		foreach (var key in keys)
		{
			CurrentFiles.Remove(key);
		}

		UpdateCacheInMemory();

		var toRemove = watchers.Where(x => x.Path.Contains(path)).ToList();

		foreach (var watcher in toRemove)
			watcher.Dispose();

		watchers.RemoveAll(x => x.Path.Contains(path));
	}


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
		foreach (var (metaData, data) in CurrentFiles)
		{
			writer.WriteLine(metaData.Path + "|" + metaData.DateTime);
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
