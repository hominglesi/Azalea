using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;

namespace Azalea.Web.Rendering;

public class WebGLVertexBuffer : Disposable
{
	private uint _handle;

	public WebGLVertexBuffer()
	{
		_handle = GL.GenBuffer();
	}

	// TODO: Call implementation
	public void Bind() => throw new NotImplementedException();
	// TODO: Call implementation
	public void Unbind() => throw new NotImplementedException();

	public void SetData(float[] data, GLUsageHint hint)
		=> SetData(data, data.Length, hint);

	public void SetData(float[] data, int length, GLUsageHint hint)
	{
		Bind();
		// TODO: Call implementation
		// GL.BufferData(GLBufferType.Array, data, length, hint);
	}

	protected override void OnDispose()
	{
		// TODO: Call implementation
		// GL.DeleteBuffer(_handle);
	}
}
