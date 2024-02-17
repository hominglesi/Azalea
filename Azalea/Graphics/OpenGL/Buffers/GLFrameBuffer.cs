using Azalea.Graphics.OpenGL.Enums;

namespace Azalea.Graphics.OpenGL.Buffers;
public class GLFramebuffer : GLBuffer
{
	private uint _texture;

	public GLFramebuffer()
		: base(GLBufferType.Frame)
	{
		Bind();

		_texture = GL.GenTexture();
	}

	public void UpdateTexture(int samples, GLColorFormat internalFormat, Vector2Int size, bool fixedSampleLocations)
	{
		Bind();

		GL.BindTexture(GLTextureType.Texture2DMultisample, _texture);
		GL.TexImage2DMultisample(GLTextureType.Texture2DMultisample, samples, internalFormat, size.X, size.Y, fixedSampleLocations);
		GL.BindTexture(GLTextureType.Texture2DMultisample, 0);
		GL.FramebufferTexture2D(GLBufferType.Frame, GLAttachment.Color0, GLTextureType.Texture2DMultisample, _texture, 0);
	}

	public bool IsComplete()
	{
		Bind();
		return GL.CheckFramebufferStatus(GLBufferType.Frame) == GLFramebufferStatus.Complete;
	}

	protected override uint CreateBuffer()
		=> GL.GenFramebuffer();

	protected override void DeleteBuffer()
		=> GL.DeleteFramebuffer(Handle);

	public override void Bind()
		=> GL.BindFramebuffer(Type, Handle);

	public override void Unbind()
		=> GL.BindFramebuffer(Type, 0);
}
