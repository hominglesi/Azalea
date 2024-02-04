using System.Collections.Generic;
using System.Text;

namespace Azalea.IO.Configs;
public static class ConfigParser
{
	public static string Format(Dictionary<string, string> keyValuePairs)
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

	public static Dictionary<string, string> Parse(string data)
	{
		var output = new Dictionary<string, string>();
		var lines = data.Split('\n');
		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var args = line.Split('=', 2);
			output.Add(args[0], args[1]);
		}
		return output;
	}
}
