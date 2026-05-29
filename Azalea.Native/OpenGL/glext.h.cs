namespace Azalea.Native.OpenGL;

// Extention Constants and Functions defined in new versions of OpenGL
// Defined here: https://registry.khronos.org/OpenGL/api/GL/glext.h
// To simplify method calls GL_ and gl prefixes are stripped
public static partial class GL
{
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GL/glext.h">Official Documentation</see></summary>
	public const int DRAW_FRAMEBUFFER = 0x8CA9;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GL/glext.h">Official Documentation</see></summary>
	public const int FRAMEBUFFER = 0x8D40;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GL/glext.h">Official Documentation</see></summary>
	public const int READ_FRAMEBUFFER = 0x8CA8;
}
