using Azalea.Graphics.OpenGL.Enums;
using System;

namespace Azalea.Graphics.OpenGL.Buffers;
public class GLUniformBuffer : GLBuffer
{
	public GLUniformBuffer()
		: base(GLBufferType.Uniform) { }

	public void BindBufferBase(uint index)
	{
		GL.BindBufferBase(Type, index, Handle);
	}

	public void BindBufferRange(uint index, int offset, int size)
	{
		GL.BindBufferRange(Type, index, Handle, (IntPtr)offset, (IntPtr)size);
	}
}
