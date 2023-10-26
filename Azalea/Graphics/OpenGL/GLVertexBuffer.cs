using Azalea.Graphics.OpenGL.Enums;
using System;

namespace Azalea.Graphics.OpenGL;
public class GLVertexBuffer : IDisposable
{
	private uint _handle;

	public GLVertexBuffer()
	{
		_handle = GL.GenBuffer();
	}

	public void Bind() => GL.BindBuffer(GLBufferType.Array, _handle);
	public void Unbind() => GL.BindBuffer(GLBufferType.Array, 0);

	public void SetData(float[] data, GLUsageHint hint)
		=> SetData(data, data.Length, hint);

	public void SetData(float[] data, int length, GLUsageHint hint)
	{
		Bind();
		GL.BufferData(GLBufferType.Array, data, length, hint);
	}

	#region Disposing
	private bool _disposed;
	~GLVertexBuffer() => Dispose(false);

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
