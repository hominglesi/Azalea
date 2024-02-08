using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;

namespace Azalea.Graphics.OpenGL;
public class GLFramebuffer : Disposable
{
	public uint Handle { get; init; }

	public GLFramebuffer()
	{
		Handle = GL.GenFramebuffer();
	}

	public void Bind() => GL.BindFramebuffer(GLBufferType.Framebuffer, Handle);
	public void Unbind() => GL.BindFramebuffer(GLBufferType.Framebuffer, 0);

	protected override void OnDispose()
	{
		GL.DeleteFramebuffer(Handle);
	}
}
