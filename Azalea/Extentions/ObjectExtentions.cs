using Azalea.Utils;
using Azalea.Utils.Proxies;
using System;

namespace Azalea.Extentions;

public static class ObjectExtentions
{
	public static T AsNotNull<T>(this T? obj) => obj!;

	public static IProxy<T> CreateProxy<T>(this object original, string propertyName,
		Action<T>? indirectSet = null)
		=> new Proxy<T>(original!, propertyName, indirectSet);
}
