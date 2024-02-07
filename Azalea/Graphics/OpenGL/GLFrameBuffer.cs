using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;

namespace Azalea.Graphics.OpenGL;
public class GLFrameBuffer : Disposable
{
	private uint _handle;

	public GLFrameBuffer()
	{
		_handle = GL.GenFramebuffer();
	}

	public void Bind() => GL.BindFramebuffer(GLBufferType.FrameBuffer, _handle);
	public void Unbind() => GL.BindFramebuffer(GLBufferType.FrameBuffer, 0);

	protected override void OnDispose()
	{
		GL.DeleteFramebuffer(_handle);
	}
}
