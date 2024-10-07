using System.Collections.Generic;
using System.Text;

namespace Azalea.IO.Configs;
internal static class ConfigParser
{
	internal static string Format(Dictionary<string, string> keyValuePairs)
	{
		StringBuilder output = new();
		foreach (var (key, value) in keyValuePairs)
		{
			output.Append(key);
			output.Append('=');
			output.Append(value);
			output.Append('\n');
		}
		return output.ToString();
	}

	internal static void Parse(string data, ref Dictionary<string, string> targetDictionary)
	{
		var lines = data.Split('\n');
		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var args = line.Split('=', 2);
			targetDictionary.Add(args[0], args[1]);
		}
	}
}
