using Azalea.Graphics.OpenGL.Buffers;
using Azalea.Utils;

namespace Azalea.Graphics.OpenGL;
public class GLVertexArray : Disposable
{
	public uint Handle { get; init; }

	public GLVertexArray()
	{
		Handle = GL.GenVertexArray();
	}

	public void Bind()
		=> GL.BindVertexArray(Handle);
	public void Unbind()
		=> GL.BindVertexArray(0);

	public void AddBuffer(GLVertexBuffer buffer, GLVertexBufferLayout layout)
	{
		Bind();
		buffer.Bind();

		var offset = 0;
		for (uint i = 0; i < layout.Elements.Count; i++)
		{
			var element = layout.Elements[(int)i];
			GL.EnableVertexAttribArray(i);
			GL.VertexAttribPointer(i, element, layout.Stride, offset);

			offset += element.Count * GLExtentions.SizeFromGLDataType(element.Type);
		}
	}

	protected override void OnDispose()
	{
		GL.DeleteVertexArray(Handle);
	}
}
