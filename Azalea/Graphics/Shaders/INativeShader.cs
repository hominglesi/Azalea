namespace Azalea.Graphics.Shaders;
internal interface INativeShader
{
	public void SetUniform(string name, float f0, float f1);
	public void SetUniform(string name, float f);
}
