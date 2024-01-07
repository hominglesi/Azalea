using System.Text;

namespace Azalea.Utils;
public static class TextUtils
{
	private static string[] words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
		"adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
		"tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

	public static string GenerateLoremIpsum(int wordCount)
	{
		var builder = new StringBuilder();

		for (int i = 0; i < wordCount; i++)
		{
			var word = words.Random();

			if (i == 0)
			{
				word = char.ToUpper(word[0]) + word[1..];
			}

			builder.Append(word);

			if (i < wordCount - 1)
				builder.Append(' ');
		}

		return builder.ToString();
	}
}
