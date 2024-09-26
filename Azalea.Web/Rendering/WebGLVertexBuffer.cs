using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;

namespace Azalea.Web.Rendering;

public class WebGLVertexBuffer : Disposable
{
	private object _handle;

	public WebGLVertexBuffer()
	{
		_handle = WebGL.CreateBuffer();
	}

	public void Bind() => WebGL.BindBuffer(GLBufferType.Array, _handle);

	public void Unbind() => throw new NotImplementedException();

	public void SetData(double[] data, GLUsageHint hint)
		=> SetData(data, data.Length, hint);

	public void SetData(double[] data, int length, GLUsageHint hint)
	{
		Bind();
		//WebGL.BufferData(GLBufferType.Array, data, hint);
	}

	protected override void OnDispose()
	{
		WebGL.DeleteBuffer(_handle);
	}
}
