using Azalea.IO.Resources;
using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Configs;
public static class Config
{
	private static string configName = "config.cfg";

	private static Dictionary<string, string> _dictionary;

	public static void Load()
	{
		if (Assets.PersistentStore.Exsists(configName) == false)
		{
			_dictionary = new Dictionary<string, string>();
			return;
		}

		var data = Assets.PersistentStore.GetText(configName)!;
		_dictionary = ConfigParser.Parse(data);
	}

	public static void Save()
	{
		var stream = Assets.PersistentStore.GetOrCreateStream(configName);
		var writer = new StreamWriter(stream);
		writer.Write(ConfigParser.Format(_dictionary));
		writer.Close();
		stream.Close();
	}

	public static string? GetValue(string key)
	{
		if (_dictionary.ContainsKey(key) == false)
			return null;

		return _dictionary[key];
	}

	public static void SetValue(string key, string value)
	{
		_dictionary[key] = value;
	}


}
