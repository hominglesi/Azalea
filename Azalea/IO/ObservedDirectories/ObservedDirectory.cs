using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azalea.IO.ObservedDirectories;
public class ObservedDirectory : Disposable
{
	private readonly List<FileSystemWatcher> watchers = [];
	private readonly Dictionary<DirectoryFileData, string> _currentFiles = [];
	private readonly Dictionary<DirectoryFileData, string> _cacheFiles = [];
	private readonly List<string> _allPaths = [];
	private readonly string _cachePath;
	private bool _started = false;

	public ObservedDirectory(IEnumerable<string> paths, string cachePath)
	{
		_cachePath = cachePath;
		_allPaths.AddRange(paths);
	}

	public void Start()
	{
		if (_started) return;
		_started = true;

		foreach (var path in _allPaths)
			if (Directory.Exists(path) == false)
				throw new Exception("The provided directory does not exists");

		var stream = Assets.PersistentStore.GetOrCreateStream(_cachePath);
		using var reader = new StreamReader(stream);
		while (reader.Peek() != -1)
		{
			var metadataLine = reader.ReadLine();
			var data = reader.ReadLine();
			if (metadataLine is null || data is null)
				return;

			var metadata = parseMetadata(metadataLine);

			_cacheFiles.Add(new(metadata.Path, metadata.DateTime), data);
			OnLoaded?.Invoke(metadata.Path, data);
		}

		processPaths(_allPaths);

		foreach (var cacheFile in _cacheFiles)
			if (_currentFiles.ContainsKey(cacheFile.Key) == false)
				OnDeleted?.Invoke(cacheFile.Key.Path);
	}


	private void processPaths(IEnumerable<string> paths)
	{
		foreach (string path in paths)
		{
			if (!_allPaths.Contains(path))
				_allPaths.Add(path);

			foreach (string metadataLine in getMetadataItems(path))
			{
				var metadata = parseMetadata(metadataLine);

				_currentFiles.Add(metadata, getSerializedData(metadata.Path));

				if (_cacheFiles.ContainsKey(metadata) == false)
					OnCreated?.Invoke(metadata.Path);
			}

			createWatcherOnPath(path);
		}
	}

	private void createWatcherOnPath(string path)
	{
		var watcher = new FileSystemWatcher(path);
		watcher.NotifyFilter = NotifyFilters.Attributes
								| NotifyFilters.CreationTime
								| NotifyFilters.DirectoryName
								| NotifyFilters.FileName
								| NotifyFilters.LastWrite
								| NotifyFilters.Size;

		watcher.IncludeSubdirectories = true;

		watcher.Created += (ob, args) =>
		{
			var metaData = new DirectoryFileData(args.FullPath, File.GetLastWriteTime(args.FullPath));

			_currentFiles.Add(metaData, getSerializedData(args.FullPath));
			OnCreated?.Invoke(metaData.Path);
		};

		watcher.Changed += (ob, args) =>
		{
			// We don't care if a directory is changed
			if (Directory.Exists(args.FullPath))
				return;

			foreach (var file in _currentFiles)
				if (file.Key.Path == args.FullPath)
				{
					_currentFiles.Remove(file.Key);
					break;
				}

			var metaData = new DirectoryFileData(args.FullPath, File.GetLastWriteTime(args.FullPath));

			_currentFiles[metaData] = getSerializedData(args.FullPath);
			OnModified?.Invoke(metaData.Path);
		};

		watcher.Deleted += (ob, args) =>
		{
			var metaData = new DirectoryFileData(args.FullPath, File.GetLastWriteTime(args.FullPath));

			_currentFiles.Remove(metaData);
			OnDeleted?.Invoke(metaData.Path);
		};

		watcher.EnableRaisingEvents = true;
		watchers.Add(watcher);
	}

	private static DirectoryFileData parseMetadata(string metadata)
		=> new(metadata.Split('|')[0], DateTime.Parse(metadata.Split('|')[1]));

	protected override void OnDispose()
	{
		foreach (var watcher in watchers)
			watcher.Dispose();

		watchers.Clear();
	}

	public void AddPath(string path) => processPaths([path]);
	public void RemovePath(string path)
	{
		if (_allPaths.Contains(path) == false)
			return;

		_allPaths.Remove(path);

		var keys = _currentFiles
			.Where(x => x.Key.Path.StartsWith(path.Replace('/', '\\')))
			.Select(x => x.Key)
			.ToList();

		foreach (var key in keys)
		{
			_currentFiles.Remove(key);
			OnDeleted?.Invoke(key.Path);
		}

		foreach (var watcher in watchers)
			if (watcher.Path == path)
			{
				watchers.Remove(watcher);
				watcher.Dispose();
				break;
			}
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

	private string getSerializedData(string path)
		=> SerializeFileMethod is not null ? SerializeFileMethod(path) : "";

	public void SaveCache()
	{
		var stream = Assets.PersistentStore.GetOrCreateStream(_cachePath);
		using var writer = new StreamWriter(stream);
		foreach (var (metaData, data) in _currentFiles)
		{
			writer.WriteLine(metaData.Path + "|" + metaData.DateTime);
			writer.WriteLine(data);
		}
	}

	private IEnumerable<string> getMetadataItems(string path = "")
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
				foreach (var file in getMetadataItems(directory.FullName))
					yield return file;

			foreach (var file in directoryInfo.EnumerateFiles())
				yield return $"{file.FullName}|{file.LastWriteTime}";
		}
	}

	private struct DirectoryFileData(string path, DateTime dateTime)
	{
		public string Path = path;
		public DateTime DateTime = dateTime;
	}
}
