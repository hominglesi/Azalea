using System.Collections.Generic;

namespace Azalea.Extentions.IEnumerableExtentions;

public static class EnumerableExtentions
{
	public static IEnumerable<T> Yield<T>(this T item) => new[] { item };
}
