using System;
using System.Numerics;

namespace Azalea.IO.Configs;
public interface IConfigProvider
{
	internal void Save();

	public bool ContainsKey(string key);

	public void Set(string key, string value);
	public string? Get(string key);

	public void Set(string key, int value);
	public int? GetInt(string key);

	public void Set(string key, float value);
	public float? GetFloat(string key);

	public void Set(string key, bool value);
	public bool? GetBool(string key);

	public void Set(string key, Vector2 value);
	public Vector2? GetVector2(string key);

	public void Set(string key, Vector2Int value);
	public Vector2Int? GetVector2Int(string key);

	public void Set<T>(string key, T value) where T : struct, Enum;
	public T? GetEnum<T>(string key) where T : struct, Enum;
}
