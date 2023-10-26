using System;

namespace Azalea.Graphics.OpenGL;
public class GLVertexArray : IDisposable
{
	private uint _handle;

	public GLVertexArray()
	{
		_handle = GL.GenVertexArray();
	}

	public void Bind() => GL.BindVertexArray(_handle);
	public void Unbind() => GL.BindVertexArray(0);

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

	#region Disposing
	private bool _disposed;
	~GLVertexArray() => Dispose(false);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed) return;

		if (disposing)
		{
			GL.DeleteVertexArray(_handle);
		}

		_disposed = true;
	}
	#endregion
}
