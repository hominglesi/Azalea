using Azalea.Audio;
using Azalea.Graphics.Textures;
using System;
using System.IO;

namespace Azalea.IO.Resources;

public static partial class Assets
{
	public static IResourceStore MainStore => _mainStore;
	private static ResourceStoreContainer _mainStore;

	public static IResourceStore FileSystem => _fileSystemStore;
	private static FileSystemStore _fileSystemStore;


	public static Action? OnDispose;

	static Assets()
	{
		_mainStore = new ResourceStoreContainer();
		AddToMainStore(new NamespacedResourceStore(
			new AssemblyResourceStore(typeof(AzaleaGame).Assembly), "Resources"));

		_fileSystemStore = new FileSystemStore();
	}

	public static void AddToMainStore(IResourceStore store) => _mainStore.AddStore(store);

	public static Stream? GetStream(string path) => MainStore.GetStream(path);
	public static Texture GetTexture(string path) => MainStore.GetTexture(path);
	public static Texture MissingTexture => GetTexture("Textures/missing-texture.png");
	public static string? GetText(string path) => MainStore.GetText(path);
	public static Sound GetSound(string path) => MainStore.GetSound(path);
	public static void AddFont(string path, string name) => MainStore.AddFont(path, name);

	internal static void DisposeAssets()
	{
		OnDispose?.Invoke();
	}
}
