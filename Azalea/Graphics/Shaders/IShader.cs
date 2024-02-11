using Azalea.Graphics.Colors;
using System.Numerics;

namespace Azalea.Graphics.Shaders;
internal interface IShader
{
	void Bind();

	void Unbind();

	void SetUniform(string name, int i);
	void SetUniform(string name, int[] array);
	void SetUniform(string name, Vector4 vec4);
	void SetUniform(string name, Color color);
	void SetUniform(string name, Matrix4x4 matrix);

}
