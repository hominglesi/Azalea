using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.IO;
internal static partial class WebLocalStorage
{
	private const string ImportString = "JSImports";

	[JSImport("WebLocalStorage.Clear", ImportString)]
	internal static partial void Clear();

	[JSImport("WebLocalStorage.GetItem", ImportString)]
	internal static partial string GetItem(string key);

	[JSImport("WebLocalStorage.GetLength", ImportString)]
	internal static partial int GetLength();

	[JSImport("WebLocalStorage.Key", ImportString)]
	internal static partial string Key(int index);

	[JSImport("WebLocalStorage.RemoveItem", ImportString)]
	internal static partial void RemoveItem(string key);

	[JSImport("WebLocalStorage.SetItem", ImportString)]
	internal static partial void SetItem(string key, string value);

}
