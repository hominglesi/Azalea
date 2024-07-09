using System;

namespace Azalea.VisualTests;
internal class VisualTestUtils
{
	public static string GetTestDisplayName(Type testType)
		=> GetTestDisplayName(testType.Name);

	public static string GetTestDisplayName(string name)
	{
		ReadOnlySpan<char> nameSpan = name.AsSpan();

		var lastDot = nameSpan.LastIndexOf('.');
		if (lastDot != -1)
			nameSpan = nameSpan[(lastDot + 1)..];

		if (nameSpan.EndsWith("Test"))
			nameSpan = nameSpan[..^4];

		else if (nameSpan.EndsWith("Tests"))
			nameSpan = nameSpan[..^5];

		return nameSpan.ToString();
	}
}
