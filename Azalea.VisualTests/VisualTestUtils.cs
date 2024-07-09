using System;

namespace Azalea.VisualTests;
internal class VisualTestUtils
{
	public static string GetTestDisplayName(string fullName)
	{
		var startIndex = fullName.LastIndexOf('.') + 1;
		ReadOnlySpan<char> name = fullName.AsSpan(startIndex, fullName.Length - startIndex);

		if (name.EndsWith("Test"))
			name = name[..^4];

		return name.ToString();
	}
}
