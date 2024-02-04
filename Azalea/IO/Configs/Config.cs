using Azalea.Extentions;
using Azalea.IO.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Azalea.IO.Configs;
public static class Config
{
	private static string configName = "config.cfg";

	private static Dictionary<string, string> _dictionary;

	public static void Load()
	{
		if (Assets.PersistentStore.Exists(configName) == false)
		{
			_dictionary = new Dictionary<string, string>();
			return;
		}

		var data = Assets.PersistentStore.GetText(configName)!;
		_dictionary = ConfigParser.Parse(data);
	}

	public static void Save()
	{
		Assets.PersistentStore.Delete(configName);
		var stream = Assets.PersistentStore.GetOrCreateStream(configName);
		var writer = new StreamWriter(stream);
		writer.Write(ConfigParser.Format(_dictionary));
		writer.Close();
		stream.Close();
	}

	public static bool ContainsKey(string key)
		=> _dictionary.ContainsKey(key);

	public static string? GetValue(string key)
	{
		if (_dictionary.ContainsKey(key) == false)
			return null;

		return _dictionary[key];
	}

	public static void SetValue(string key, string value)
		=> _dictionary[key] = value;

	public static int? GetValueInt(string key)
	{
		var value = GetValue(key);
		if (value is null) return null;

		return int.Parse(value);
	}

	public static void SetValue(string key, int value)
		=> SetValue(key, value.ToString());

	public static float? GetValueFloat(string key)
	{
		var value = GetValue(key);
		if (value is null) return null;

		return float.Parse(value);
	}

	public static void SetValue(string key, float value)
		=> SetValue(key, value.ToString());

	public static bool? GetValueBool(string key)
	{
		var value = GetValue(key);
		if (value is null) return null;

		return bool.Parse(value);
	}

	public static void SetValue(string key, bool value)
		=> SetValue(key, value.ToString());

	public static Vector2? GetValueVector2(string key)
	{
		var value = GetValue(key);
		if (value is null) return null;

		return Vector2Extentions.Parse(value);
	}

	public static void SetValue(string key, Vector2 value)
		=> SetValue(key, $"{value.X}:{value.Y}");

	public static Vector2Int? GetValueVector2Int(string key)
	{
		var value = GetValue(key);
		if (value is null) return null;

		return Vector2Int.Parse(value);
	}

	public static void SetValue(string key, Vector2Int value)
		=> SetValue(key, $"{value.X}:{value.Y}");

	public static T GetValueEnum<T>(string key)
		where T : struct, Enum
	{
		//We can't return null with enums so we can assume the key exists
		//and let the user check before calling this method
		int value = GetValueInt(key) ?? -1;
		return Unsafe.As<int, T>(ref value);
	}

	public static void SetValue<T>(string key, T value)
		where T : struct, Enum
		=> SetValue(key, Unsafe.As<T, int>(ref value));
}
