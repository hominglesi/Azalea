using Azalea.Graphics.OpenGL.Enums;
using System.Runtime.InteropServices;

namespace Azalea.Graphics.OpenGL;
internal static class GL
{
	private const string LibraryPath = "opengl32.dll";

	[DllImport(LibraryPath, EntryPoint = "glClear")]
	public static extern void Clear(GLBufferBits bufferBits);

	[DllImport(LibraryPath, EntryPoint = "glClearColor")]
	public static extern void ClearColor(float red, float green, float blue, float alpha);

	[DllImport(LibraryPath, EntryPoint = "glBegin")]
	public static extern void Begin(GLBeginMode mode);

	[DllImport(LibraryPath, EntryPoint = "glEnd")]
	public static extern void End();

	#region GLVertex

	[DllImport(LibraryPath, EntryPoint = "glVertex2f")]
	public static extern void Vertex2f(float x, float y);

	#endregion
}
