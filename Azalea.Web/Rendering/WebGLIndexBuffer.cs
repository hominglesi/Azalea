using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;

namespace Azalea.Web.Rendering;

public class WebGLIndexBuffer : Disposable
{
	private uint _handle;

	public WebGLIndexBuffer()
	{
		// Call Implementation
		// 
	}

	public void SetData(uint[] data, GLUsageHint hint)
	{
		Bind();

		// Call Implementation
		// GL.BufferData(GLBufferType.ElementArray, data, hint);
	}

	// Call Implementation
	public void Bind() => throw new NotImplementedException();

	// Call Implementation
	public void Unbind() => throw new NotImplementedException();

	protected override void OnDispose()
	{
		// TODO: Call implementation
		// 
	}
}
