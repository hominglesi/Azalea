using Azalea.Platform;
using System;
using System.Numerics;

namespace Azalea.IO.Configs;
public static class Config
{
	private static IConfigProvider _provider => GameHost.Main.ConfigProvider
		?? throw new Exception("Config must be set up to be able to use it.");

	public static bool ContainsKey(string key) => _provider.ContainsKey(key);

	public static void Set(string key, string value) => _provider.Set(key, value);
	public static string? Get(string key) => _provider.Get(key);

	public static void Set(string key, int value) => _provider.Set(key, value);
	public static int? GetInt(string key) => _provider.GetInt(key);

	public static void Set(string key, float value) => _provider.Set(key, value);
	public static float? GetFloat(string key) => _provider.GetFloat(key);

	public static void Set(string key, bool value) => _provider.Set(key, value);
	public static bool? GetBool(string key) => _provider.GetBool(key);

	public static void Set(string key, Vector2 value) => _provider.Set(key, value);
	public static Vector2? GetVector2(string key) => _provider.GetVector2(key);

	public static void Set(string key, Vector2Int value) => _provider.Set(key, value);
	public static Vector2Int? GetVector2Int(string key) => _provider.GetVector2Int(key);

	public static void Set<T>(string key, T value)
		where T : struct, Enum => _provider.Set(key, value);
	public static T? GetEnum<T>(string key)
		where T : struct, Enum => _provider.GetEnum<T>(key);
}
