using System.Reflection;
using System.Runtime.InteropServices;

namespace Azalea.Native.Utils;

internal static class ImportResolving
{
	private readonly static Dictionary<string, string> _translations = [];

	internal static void AddDirectoryTranslation(string key, string path)
	{
		assureSet();

		var directoryPath = Path.Combine(AppContext.BaseDirectory, path);
		_translations.Add(key, directoryPath);
	}

	private static bool _set = false;

	private static void assureSet()
	{
		if (_set == true) return;

		NativeLibrary.SetDllImportResolver(typeof(ImportResolving).Assembly, resolve);
		_set = true;
	}

	private static nint resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
	{
		var path = libraryName;

		if (_translations.TryGetValue(libraryName, out var libraryPath))
			path = libraryPath;

		if (path is null)
			return nint.Zero;

		return NativeLibrary.Load(path);
	}
}
