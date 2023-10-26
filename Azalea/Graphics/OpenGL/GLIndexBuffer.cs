using Azalea.Graphics.OpenGL.Enums;
using System;

namespace Azalea.Graphics.OpenGL;
public class GLIndexBuffer : IDisposable
{
	private uint _handle;

	public GLIndexBuffer()
	{
		_handle = GL.GenBuffer();
	}
	public void SetData(uint[] data, GLUsageHint hint)
	{
		Bind();
		GL.BufferData(GLBufferType.ElementArray, data, hint);
	}
	public void Bind() => GL.BindBuffer(GLBufferType.ElementArray, _handle);
	public void Unbind() => GL.BindBuffer(GLBufferType.ElementArray, 0);

	#region Disposing
	private bool _disposed;
	~GLIndexBuffer() => Dispose(false);

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
			GL.DeleteBuffer(_handle);
		}

		_disposed = true;
	}
	#endregion
}
