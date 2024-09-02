﻿using Azalea.Audio;
using Azalea.Graphics.Textures;
using Azalea.Utils;
using System;
using System.IO;
using System.Reflection;

namespace Azalea.IO.Resources;

public static partial class Assets
{
	/// <summary>
	/// The main store containing all the games embedded resources.
	/// </summary>
	public static IResourceStore MainStore => _mainStore;
	private static ResourceStoreContainer _mainStore;

	/// <summary>
	/// A store for accessing resources on the system with an absolute path.
	/// </summary>
	public static IResourceStore FileSystemStore => _fileSystemStore;
	private static FileSystemStore _fileSystemStore;

	/// <summary>
	/// A store for storing and accessing persistent game data. Must be set-up before
	/// using by calling the <see cref="SetupPersistentStore(string)"/> method.
	/// </summary>
	public static Storage PersistentStore => _persistentStore
		?? throw new Exception("A persistent store has not been set up. Please use Assets.SetupPersistentStore before trying to access it");
	private static Storage? _persistentStore;

	public static Storage ReflectedStore => _reflectedStore
		?? throw new Exception("A reflected store has not been set up. Please use Assets.SetupReflectedStore before trying to access it");
	private static Storage? _reflectedStore;

	static Assets()
	{
		_mainStore = new ResourceStoreContainer();
		AddToMainStore(new NamespacedResourceStore(
			new EmbeddedResourceStore(typeof(AzaleaGame).Assembly), "Resources"));

		_fileSystemStore = new FileSystemStore();
	}

	/// <summary>
	/// Specifies the folder name that is used to store game data.
	/// For windows this is stored in the /AppData/Roaming directory.
	/// </summary>
	public static void SetupPersistentStore(string folderName)
	{
		var path = FileSystemUtils.CombinePaths(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folderName);
		_persistentStore = new Storage(path);
	}

	public static void SetupReflectedStore(string reflectedPath)
	{
		var exePath = Assembly.GetEntryAssembly()!.Location;
		var path = FileSystemUtils.CombinePaths(exePath, reflectedPath);
		_reflectedStore = new Storage(path);
	}

	/// <summary>
	/// Returned texture used when a requested texture doesn't exist.
	/// </summary>
	public static Texture MissingTexture => GetTexture("Textures/missing-texture.png");

	/// <summary>
	/// Makes a resource store available for use from the main resource store.
	/// </summary>
	public static void AddToMainStore(IResourceStore store) => _mainStore.AddStore(store);

	/// <summary>
	/// Gets a stream from the main resource store.
	/// </summary>
	public static Stream? GetStream(string path) => MainStore.GetStream(path);

	/// <summary>
	/// Gets a texture from the main resource store.
	/// </summary>
	public static Texture GetTexture(string path) => MainStore.GetTexture(path);

	/// <summary>
	/// Gets text from the main resource store.
	/// </summary>
	public static string? GetText(string path) => MainStore.GetText(path);

	/// <summary>
	/// Gets a sound from the main resource store.
	/// </summary>
	public static Sound GetSound(string path) => MainStore.GetSound(path);

	/// <summary>
	/// Adds a font to the main store to be used when font name is specified.
	/// </summary>
	public static void AddFont(string path, string fontName) => MainStore.AddFont(path, fontName);
}
