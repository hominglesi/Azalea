using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;

namespace Azalea.Graphics.OpenGL;
public class GLVertexBuffer : Disposable
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

	protected override void OnDispose()
	{
		GL.DeleteBuffer(_handle);
	}
}
