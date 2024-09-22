using Azalea.Graphics.OpenGL;
using Azalea.Utils;
using System;

namespace Azalea.Web.Rendering;

public class WebGLVertexArray : Disposable
{
	private uint _handle;

	public WebGLVertexArray()
	{
		_handle = GL.GenVertexArray();
	}

	// Call Implementation
	public void Bind() => throw new NotImplementedException();
	// Call Implementation
	public void Unbind() => throw new NotImplementedException();

	public void AddBuffer(WebGLVertexBuffer buffer, GLVertexBufferLayout layout)
	{
		Bind();
		buffer.Bind();

		var offset = 0;
		for (uint i = 0; i < layout.Elements.Count; i++)
		{
			var element = layout.Elements[(int)i];
			// Call Implementation
			// GL.EnableVertexAttribArray(i);
			// GL.VertexAttribPointer(i, element, layout.Stride, offset);

			offset += element.Count * GLExtentions.SizeFromGLDataType(element.Type);
		}
	}

	protected override void OnDispose()
	{
		// Call Implementation
		// GL.DeleteVertexArray(_handle);
	}
}
