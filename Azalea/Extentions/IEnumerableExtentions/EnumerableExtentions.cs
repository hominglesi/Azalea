using System;
using System.Collections.Generic;

namespace Azalea.Extentions.IEnumerableExtentions;

public static class EnumerableExtentions
{
	public static IEnumerable<T> Yield<T>(this T item) => new[] { item };

	public static void LogAllValues(this IEnumerable<string> enumerable)
	{
		foreach (var item in enumerable)
		{
			Console.WriteLine(item);
		}
	}
}
