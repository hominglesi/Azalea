using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;

namespace Azalea.Graphics.OpenGL;
public class GLIndexBuffer : Disposable
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

	protected override void OnDispose()
	{
		GL.DeleteBuffer(_handle);
	}
}
