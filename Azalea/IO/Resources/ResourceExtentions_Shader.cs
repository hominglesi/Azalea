using Azalea.Graphics.Rendering;
using Azalea.Graphics.Shaders;
using System.IO;

namespace Azalea.IO.Resources;

public static partial class ResourceStoreExtentions
{
	private static ResourceCache<Shader> _shaderCache = new();

	public static Shader GetShader(this IResourceStore store, string vertexShaderPath, string fragmentShaderPath)
	{
		var combinedPath = $"{vertexShaderPath}+{fragmentShaderPath}";

		if (_shaderCache.TryGetValue(store, combinedPath, out var cached))
			return cached;

		var vertexShaderCode = store.GetText(vertexShaderPath)
			?? throw new FileNotFoundException("Vertex shader could not be found.");
		var fragmentShaderCode = store.GetText(fragmentShaderPath)
			?? throw new FileNotFoundException("Fragment shader could not be found.");

		var shader = Renderer.CreateShader(vertexShaderCode, fragmentShaderCode);

		_shaderCache.AddValue(store, combinedPath, shader);

		return shader;
	}
}
