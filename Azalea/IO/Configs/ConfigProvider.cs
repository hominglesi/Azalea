using Azalea.Extentions;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.IO.Configs;
internal abstract class ConfigProvider : IConfigProvider
{
	protected Dictionary<string, string> Dictionary = new();

	public abstract void Save();

	public bool ContainsKey(string key) => Dictionary.ContainsKey(key);

	public void Set(string key, string value) => Dictionary[key] = value;
	public string? Get(string key)
	{
		if (Dictionary.TryGetValue(key, out var value) == false)
			return null;

		return value;
	}

	private T? getConverted<T>(string key, Func<string, T> converterFunction)
		where T : struct
	{
		if (Dictionary.TryGetValue(key, out var value) == false)
			return null;

		return converterFunction(value);
	}

	public void Set(string key, int value) => Set(key, value.ToString());
	public int? GetInt(string key) => getConverted(key, (value) => int.Parse(value));

	public void Set(string key, float value) => Set(key, value.ToString());
	public float? GetFloat(string key) => getConverted(key, (value) => float.Parse(value));

	public void Set(string key, bool value) => Set(key, value.ToString());
	public bool? GetBool(string key) => getConverted(key, (value) => bool.Parse(value));

	public void Set(string key, Vector2 value) => Set(key, $"{value.X}:{value.Y}");
	public Vector2? GetVector2(string key) => getConverted(key, (value) => Vector2Extentions.Parse(value));

	public void Set(string key, Vector2Int value) => Set(key, $"{value.X}:{value.Y}");
	public Vector2Int? GetVector2Int(string key) => getConverted(key, (value) => Vector2Int.Parse(value));

	public void Set<T>(string key, T value)
		where T : struct, Enum => Set(key, value.ToString());
	public T? GetEnum<T>(string key)
		where T : struct, Enum
	{
		try { return getConverted(key, (value) => Enum.Parse<T>(value)); }
		catch (ArgumentException)
		{
			Console.WriteLine("Tried to read an enum that doesn't exist. Returning null.");
			return null;
		}
	}
}
