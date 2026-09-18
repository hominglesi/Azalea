using Azalea.Utils;
using System;

namespace Azalea.Extentions.ObjectExtentions;

public static class ObjectExtentions
{
	public static T AsNotNull<T>(this T? obj) => obj!;

	public static ObservableProxy<T> CreateProxy<T>(this object obj, string propertyName, Action<T>? setAction = null)
		=> new(obj!, propertyName, setAction);
}
