using Azalea.Inputs;
using Azalea.IO.ObservedDirectories;
using Azalea.Sounds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azalea.VisualTests;
internal class StorageTest : TestScene
{
	string _txtFileLocation = "";
	string _txtFileName = "storage";
	string _fullPath => _txtFileName + ".txt";

	string _songPath = "C:\\Users\\ninja\\Desktop\\MusicTest";
	List<string> _songLocations = new List<string>();
	ObservedDirectory directory;
	public StorageTest()
	{
		//List<string> textFileData = ReadSongsTextFile();
		List<string> songFolderData = GetItems(_songPath).ToList();
		//	UpdateTextFile(songFolderData);
		LoadSongs(songFolderData);

		_songLocations.Add(_songPath);
		directory = new ObservedDirectory(_songLocations.ToArray(), _fullPath, file => file[..5]);
		//directory.SerializeFileMethod = file => file;
		directory.OnCreated += (gas) =>
		{
			Console.WriteLine($"Omg, dodata je pesma {gas}");
		};


		//directory.SaveCache();
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.S).Down)
			directory.SaveCache();
	}
	/*
	private void UpdateTextFile(List<string> songData)
	{
		if (Directory.Exists(_txtFileLocation) == false)
		{
			Directory.CreateDirectory(_txtFileLocation);
		}

		if (File.Exists(_fullPath) == false)
		{
			File.Create(_fullPath);

		}
		File.WriteAllLines(_fullPath, songData);



	}
	*/
	private List<Sound> LoadSongs(List<string> songData)
	{
		List<Sound> songs = new List<Sound>();
		foreach (var song in songData)
		{

		}
		return songs;
	}

	private List<string> ReadSongsTextFile()
	{
		if (Directory.Exists(_txtFileLocation) == false)
		{
			Directory.CreateDirectory(_txtFileLocation);
		}

		if (File.Exists(_fullPath) == false)
		{
			File.Create(_fullPath).Close();
		}

		List<string> songData = new List<string>();
		songData.AddRange(File.ReadAllLines(_fullPath));
		return songData;



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
					yield return $"{file.FullName}|{file.LastWriteTime}";
			}
		}
	}
}
