namespace Azalea.Native.OpenGL;

// Core Constants and Functions that were defined after OpenGL 1.0
// They must be dynamically loaded
// Defined here: https://registry.khronos.org/OpenGL/api/GL/glcorearb.h
// To simplify method calls GL_ and gl prefixes are stripped
public static partial class GL
{
	[AttributeUsage(AttributeTargets.Delegate)]
	public sealed class OpenGLLoadedFunctionAttribute(
		string glName, string? comment = null, string? alternativeName = null) : Attribute
	{
		private string _glName = glName;
		private string? _comment = comment;
		private string? _alternativeName = alternativeName;
	}

	[OpenGLLoadedFunction("glBindBuffer", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml">Official Documentation</see>""")]
	private delegate void BindBufferDelegate(int target, uint buffer);

	[OpenGLLoadedFunction("glBindBuffer", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindFramebuffer.xhtml">Official Documentation</see>""")]
	private delegate void BindFramebufferDelegate(int target, uint framebuffer);

	[OpenGLLoadedFunction("glBindBuffer", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml">Official Documentation</see>""")]
	private delegate void BufferDataDelegate(int target, nint size, IntPtr data, int usage);

	[OpenGLLoadedFunction("glBindBuffer", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml">Official Documentation</see>""", "BufferData")]
	private delegate void BufferData2Delegate(int target, nint size, in byte data, int usage);

	[OpenGLLoadedFunction("glBindBuffer", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml">Official Documentation</see>""", "BufferData")]
	private delegate void BufferData3Delegate(int target, nint size, in float data, int usage);

	[OpenGLLoadedFunction("glCreateShader", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCreateShader.xhtml">Official Documentation</see>""")]
	private delegate uint CreateShaderDelegate(int shaderType);

	[OpenGLLoadedFunction("glCompileShader", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCompileShader.xhtml">Official Documentation</see>""")]
	private delegate uint CompileShaderDelegate(uint shader);

	[OpenGLLoadedFunction("glCompileShader", """<see href="https://registry.khronos.org/OpenGL-Refpages/es3/html/glFramebufferTexture2D.xhtml">Official Documentation</see>""")]
	private delegate nint FramebufferTexture2DDelegate(int target, int attachment, int textarget, uint texture, int level);

	[OpenGLLoadedFunction("glGenBuffers", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenBuffers.xhtml">Official Documentation</see>""")]
	private delegate void GenBuffersDelegate(int count, ref uint buffers);

	[OpenGLLoadedFunction("glGenFramebuffers", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenFramebuffers.xhtml">Official Documentation</see>""")]
	private delegate bool GenFramebuffersDelegate(int n, ref uint ids);

	[OpenGLLoadedFunction("glGetShaderiv", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenFramebuffers.xhtml">Official Documentation</see>""")]
	private delegate void GetShaderivDelegate(uint shader, int pname, ref int _params);

	[OpenGLLoadedFunction("glShaderSource", """<see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glShaderSource.xhtml">Official Documentation</see>""")]
	private delegate void ShaderSourceDelegate(uint shader, int count, ref nint _string, in int length);

	[OpenGLLoadedFunction("wglChoosePixelFormatARB", """<see href="https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_pixel_format.txt">Official Documentation</see>""")]
	private delegate bool wglChoosePixelFormatARBDelegate(nint hdc, in int piAttribIList, in float pfAttribFList, uint nMaxFormats, ref int piFormats, ref uint nNumFormats);

	[OpenGLLoadedFunction("wglCreateContextAttribsARB", """<see href="https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_create_context.txt">Official Documentation</see>""")]
	private delegate nint wglCreateContextAttribsARBDelegate(nint hDC, bool hShareContext, in int attribList);
}
