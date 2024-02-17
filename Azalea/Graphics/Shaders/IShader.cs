using Azalea.Graphics.Colors;
using System.Numerics;

namespace Azalea.Graphics.Shaders;
public interface IShader
{
	public uint Handle { get; }

	void Bind();

	void SetUniform(string name, int i);
	void SetUniform(string name, int[] array);
	void SetUniform(string name, Vector4 vec4);
	void SetUniform(string name, Color color);
	void SetUniform(string name, Matrix4x4 matrix);

}
