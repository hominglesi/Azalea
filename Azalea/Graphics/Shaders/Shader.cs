using Azalea.Graphics.Colors;
using System;
using System.Numerics;

namespace Azalea.Graphics.Shaders;

public class Shader
{
	internal virtual INativeShader NativeShader { get; }

	internal Shader(INativeShader nativeShader)
	{
		NativeShader = nativeShader ?? throw new ArgumentNullException(nameof(nativeShader));
	}

	public void SetUniform(string name, int i) => NativeShader.SetUniform(name, i);
	public void SetUniform(string name, int[] array) => NativeShader.SetUniform(name, array);
	public void SetUniform(string name, float f) => NativeShader.SetUniform(name, f);
	public void SetUniform(string name, float f0, float f1) => NativeShader.SetUniform(name, f0, f1);
	public void SetUniform(string name, float f0, float f1, float f2, float f3) => NativeShader.SetUniform(name, f0, f1, f2, f3);
	public void SetUniform(string name, Color color) => NativeShader.SetUniform(name, color);
	public void SetUniform(string name, Matrix4x4 matrix) => NativeShader.SetUniform(name, matrix);
}
