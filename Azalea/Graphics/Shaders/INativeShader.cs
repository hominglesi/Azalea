using Azalea.Graphics.Colors;
using System.Numerics;

namespace Azalea.Graphics.Shaders;
internal interface INativeShader
{
	internal void SetUniform(string name, int i);
	internal void SetUniform(string name, int[] array);
	internal void SetUniform(string name, float f);
	internal void SetUniform(string name, float f0, float f1);
	internal void SetUniform(string name, float f0, float f1, float f2, float f3);
	internal void SetUniform(string name, Color color);
	internal void SetUniform(string name, Matrix4x4 matrix);
}
