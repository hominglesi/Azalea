namespace Azalea.Platform.Rendering.OpenGL;
internal class GLTexture(uint handle) : INativeTexture
{
	public uint Handle { get; } = handle;
}
