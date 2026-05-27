namespace Azalea.Platform.Rendering.OpenGL;
internal class GLFramebuffer(GLRenderer renderer, uint handle) : Framebuffer(renderer)
{
	public uint Handle { get; } = handle;
}
