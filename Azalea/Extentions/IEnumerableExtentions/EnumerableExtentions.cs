using System;
using System.Collections.Generic;

namespace Azalea.Extentions.IEnumerableExtentions;

public static class EnumerableExtentions
{
	public static IEnumerable<T> Yield<T>(this T item) => new[] { item };

	public static void LogAllValues<T>(this IEnumerable<T> enumerable)
	{
		foreach (var item in enumerable)
		{
			Console.WriteLine(item);
		}
	}

	public static void LogAllValuesAsArray<T>(this IEnumerable<T> enumerable)
	{
		var output = "[ ";

		foreach (var item in enumerable)
			output += $"{item}, ";

		Console.WriteLine(string.Concat(output.AsSpan(0, output.Length - 2), " ]"));
	}
}
