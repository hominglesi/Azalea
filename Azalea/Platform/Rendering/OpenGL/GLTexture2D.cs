namespace Azalea.Platform.Rendering.OpenGL;
internal class GLTexture2D(GLRenderer renderer, uint handle) : Texture2D(renderer)
{
	public uint Handle { get; } = handle;
}
