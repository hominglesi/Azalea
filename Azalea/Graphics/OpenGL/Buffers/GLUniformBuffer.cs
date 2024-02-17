using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Shaders;

namespace Azalea.Graphics.OpenGL.Buffers;
public class GLUniformBuffer : GLBuffer
{
	public GLUniformBuffer()
		: base(GLBufferType.Uniform) { }

	public void SetShaderBinding(IShader shader, string name, uint binding)
	{
		var index = GL.GetUniformBlockIndex(shader.Handle, name);
		if (index != uint.MaxValue)
			GL.UniformBlockBinding(shader.Handle, index, binding);
	}

	public void BindBufferBase(uint index)
	{
		GL.BindBufferBase(Type, index, Handle);
	}
}
