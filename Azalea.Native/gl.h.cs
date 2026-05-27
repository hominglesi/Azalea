using System.Runtime.InteropServices;

namespace Azalea.Native;
public static partial class OpenGL
{
	private const string OpenGLPath = "opengl32.dll";

	#region DataType
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_BYTE = 0x1400;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_UNSIGNED_BYTE = 0x1401;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_SHORT = 0x1402;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_UNSIGNED_SHORT = 0x1403;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_FLOAT = 0x1406;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_FIXED = 0x140C;
	#endregion

	#region PixelFormat
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_ALPHA = 0x1906;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_RGB = 0x1907;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_RGBA = 0x1908;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_LUMINANCE = 0x1909;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_LUMINANCE_ALPHA = 0x190A;
	#endregion

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindTexture.xhtml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath)]
	public static partial void glBindTexture(int target, uint texture);

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/es2.0/xhtml/glClear.xml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath)]
	public static partial void glClear(int mask);

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/es2.0/xhtml/glClearColor.xml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath)]
	public static partial void glClearColor(float red, float green, float blue, float alpha);

	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_COLOR_BUFFER_BIT = 0x00004000;
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_DEPTH_BUFFER_BIT = 0x00000100;

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenTextures.xhtml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath)]
	public static partial void glGenTextures(int n, ref uint textures);

	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_STENCIL_BUFFER_BIT = 0x00000400;

	[LibraryImport(OpenGLPath)]
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexImage2D.xhtml">Official Documentation</see></summary>
	public static partial void glTexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, in byte pixels);

	[LibraryImport(OpenGLPath)]
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexImage2D.xhtml">Official Documentation</see></summary>
	public static partial void glTexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, IntPtr pixels);

	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_TEXTURE_2D = 0x0DE1;

}
