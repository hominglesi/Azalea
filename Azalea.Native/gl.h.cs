using System.Runtime.InteropServices;

namespace Azalea.Native;
public static partial class OpenGL
{
	private const string OpenGLPath = "opengl32.dll";

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
	/// <summary><see href="https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h">Official Documentation</see></summary>
	public const int GL_STENCIL_BUFFER_BIT = 0x00000400;

}
