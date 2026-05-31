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
		GL.BufferData(Native.OpenGL.GL.ELEMENT_ARRAY_BUFFER, data, hint);
	}
	public void Bind() => GL.BindBuffer(Native.OpenGL.GL.ELEMENT_ARRAY_BUFFER, _handle);
	public void Unbind() => GL.BindBuffer(Native.OpenGL.GL.ELEMENT_ARRAY_BUFFER, 0);

	protected override void OnDispose()
	{
		GL.DeleteBuffer(_handle);
	}
}
