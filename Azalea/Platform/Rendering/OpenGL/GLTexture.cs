using Azalea.Native.OpenGL;

namespace Azalea.Platform.Rendering.OpenGL;
internal class GLTexture(uint handle) : INativeTexture
{
	public uint Handle { get; } = handle;

	public int Target = GL.TEXTURE_2D;
}
