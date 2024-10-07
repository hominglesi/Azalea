using Azalea.Platform;
using System;
using System.Numerics;

namespace Azalea.IO.Configs;
public static class Config
{
	private static IConfigProvider? _instance;
	public static IConfigProvider Instance => _instance ??= GameHost.Main.ConfigProvider
		?? throw new Exception("Config must be set up to be able to use it.");

	public static bool ContainsKey(string key) => Instance.ContainsKey(key);

	public static void Set(string key, string value) => Instance.Set(key, value);
	public static string? Get(string key) => Instance.Get(key);

	public static void Set(string key, int value) => Instance.Set(key, value);
	public static int? GetInt(string key) => Instance.GetInt(key);

	public static void Set(string key, float value) => Instance.Set(key, value);
	public static float? GetFloat(string key) => Instance.GetFloat(key);

	public static void Set(string key, bool value) => Instance.Set(key, value);
	public static bool? GetBool(string key) => Instance.GetBool(key);

	public static void Set(string key, Vector2 value) => Instance.Set(key, value);
	public static Vector2? GetVector2(string key) => Instance.GetVector2(key);

	public static void Set(string key, Vector2Int value) => Instance.Set(key, value);
	public static Vector2Int? GetVector2Int(string key) => Instance.GetVector2Int(key);

	public static void Set<T>(string key, T value)
		where T : struct, Enum => Instance.Set(key, value);
	public static T? GetEnum<T>(string key)
		where T : struct, Enum => Instance.GetEnum<T>(key);
}
