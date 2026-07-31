namespace Azalea.Native.OpenGL;

// Windows-specific extentions Constants and Windows 
// Defined here: https://registry.khronos.org/OpenGL/api/GL/wglext.h
public static partial class GL
{
	public const int WGL_CONTEXT_CORE_PROFILE_BIT_ARB = 0x00000001;
	public const int WGL_DRAW_TO_WINDOW_ARB = 0x2001;
	public const int WGL_ACCELERATION_ARB = 0x2003;
	public const int WGL_SWAP_METHOD_ARB = 0x2007;
	public const int WGL_SUPPORT_OPENGL_ARB = 0x2010;
	public const int WGL_DOUBLE_BUFFER_ARB = 0x2011;
	public const int WGL_PIXEL_TYPE_ARB = 0x2013;
	public const int WGL_COLOR_BITS_ARB = 0x2014;
	public const int WGL_DEPTH_BITS_ARB = 0x2022;
	public const int WGL_STENCIL_BITS_ARB = 0x2023;
	public const int WGL_FULL_ACCELERATION_ARB = 0x2027;
	public const int WGL_SWAP_COPY_ARB = 0x2029;
	public const int WGL_TYPE_RGBA_ARB = 0x202B;
	public const int WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
	public const int WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
	public const int WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126;
}
