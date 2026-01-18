using System;

namespace Azalea.IO.ObservedDirectories;
public class ObservedDirectory
{
	public ObservedDirectory(string[] paths, string cachePath)
	{
		throw new NotImplementedException();
	}

	public void AddPath(string path) => throw new NotImplementedException();
	public void RemovePath(string path) => throw new NotImplementedException();

	/// <summary>
	/// Called when a file we have not seen before is created.
	/// </summary>
	public Action<string>? OnCreated;
	/// <summary>
	/// Called when a file we have seen before is read from the cache
	/// </summary>
	public Action<string>? OnLoaded;
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
}
