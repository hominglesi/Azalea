using Azalea.Graphics.OpenGL;
using Azalea.Utils;

namespace Azalea.Web.Rendering;

public class WebGLVertexArray : UnmanagedObject<object>
{
	protected override object CreateObject() => WebGL.CreateVertexArray();

	public void Bind() => WebGL.BindVertexArray(Handle);

	public void AddBuffer(WebGLVertexBuffer buffer, GLVertexBufferLayout layout)
	{
		Bind();
		buffer.Bind();

		var offset = 0;
		for (uint i = 0; i < layout.Elements.Count; i++)
		{
			var element = layout.Elements[(int)i];
			WebGL.EnableVertexAttribArray((int)i);
			WebGL.VertexAttribPointer((int)i, element.Count, element.Type, element.Normalized, layout.Stride, offset);

			offset += element.Count * GLExtentions.SizeFromGLDataType(element.Type);
		}
	}

	protected override void OnDispose() => WebGL.DeleteVertexArray(Handle);
}
