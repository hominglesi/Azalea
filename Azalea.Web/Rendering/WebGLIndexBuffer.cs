using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;

namespace Azalea.Web.Rendering;

public class WebGLIndexBuffer : Disposable
{
	private object _handle;

	public WebGLIndexBuffer()
	{
		_handle = WebGL.CreateBuffer();
	}

	public void SetData(int[] data, GLUsageHint hint)
	{
		Bind();
		WebGL.BufferData(GLBufferType.ElementArray, data, hint);
	}

	// Call Implementation
	public void Bind() => WebGL.BindBuffer(GLBufferType.ElementArray, _handle);

	// Call Implementation
	public void Unbind() => throw new NotImplementedException();

	protected override void OnDispose()
	{
		//WebGL.DeleteBuffer();
	}
}
