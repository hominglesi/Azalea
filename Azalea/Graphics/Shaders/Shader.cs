using System;

namespace Azalea.Graphics.Shaders;

public class Shader
{
	internal virtual INativeShader NativeShader { get; }

	internal Shader(INativeShader nativeShader)
	{
		NativeShader = nativeShader ?? throw new ArgumentNullException(nameof(nativeShader));
	}
}
