using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Shaders;
public static class ShaderLibrary
{
	private static readonly Dictionary<string, Shader> _dictionary = [];

	public static void RegisterShader(string shaderName, Shader shader)
		=> _dictionary.Add(shaderName, shader);

	public static Shader GetShader(string shaderName)
	{
		if (_dictionary.TryGetValue(shaderName, out Shader? value))
			return value;

		throw new Exception($"'{shaderName}' was never registered to the shader library");
	}
}
