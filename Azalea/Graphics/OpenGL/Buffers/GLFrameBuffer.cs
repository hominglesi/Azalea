using Azalea.Graphics.OpenGL.Enums;

namespace Azalea.Graphics.OpenGL.Buffers;
public class GLFramebuffer : GLBuffer
{
	public GLFramebuffer()
		: base(GLBufferType.Frame) { }

	protected override uint CreateBuffer()
		=> GL.GenFramebuffer();

	protected override void DeleteBuffer()
		=> GL.DeleteFramebuffer(Handle);

	public override void Bind()
		=> GL.BindFramebuffer(Type, Handle);

	public override void Unbind()
		=> GL.BindFramebuffer(Type, 0);
}
