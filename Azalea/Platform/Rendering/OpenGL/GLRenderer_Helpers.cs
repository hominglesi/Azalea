using Azalea.Native.OpenGL;
using System.Text;

namespace Azalea.Platform.Rendering.OpenGL;

internal partial class GLRenderer : PlatformRenderer
{
	private static int getUniformLocation(Program program, string uniformName)
	{
		program.AssureInitialized();

		if (program.UniformLocations is not null)
		{
			if (program.UniformLocations.TryGetValue(uniformName, out var cachedLocation))
				return cachedLocation;
		}
		else
			program.UniformLocations = [];

		var nameBytes = Encoding.UTF8.GetBytes(uniformName + "\0");
		int uniformLocation = GL.GetUniformLocation(program.Handle.Value, nameBytes);

		program.UniformLocations.Add(uniformName, uniformLocation);
		return uniformLocation;
	}
}
