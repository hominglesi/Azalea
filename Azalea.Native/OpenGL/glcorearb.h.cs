namespace Azalea.Native.OpenGL;

// Core Constants and Functions that were defined after OpenGL 1.0
// They must be dynamically loaded
// Defined here: https://registry.khronos.org/OpenGL/api/GL/glcorearb.h
// To simplify method calls GL_ and gl prefixes are stripped
public static partial class GL
{
	public const int UNSIGNED_INT = 0x1405;
	public const int LINE = 0x1B01;
	public const int POINT = 0x1B00;

	[AttributeUsage(AttributeTargets.Delegate)]
	public sealed class OpenGLLoadedFunctionAttribute(
		string? docs = null, string? overrideName = null, bool automaticPrefix = true) : Attribute
	{
		private string? _docs = docs;
		private string? _overrideName = overrideName;
		private bool _automaticPrefix = automaticPrefix;
	}

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glAttachShader.xhtml")]
	private delegate void AttachShaderDelegate(uint program, uint shader);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml")]
	private delegate void BindBufferDelegate(int target, uint buffer);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindFramebuffer.xhtml")]
	private delegate void BindFramebufferDelegate(int target, uint framebuffer);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindVertexArray.xhtml")]
	private delegate void BindVertexArrayDelegate(uint array);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferDataDelegate(int target, nint size, IntPtr data, int usage);

	[OpenGLLoadedFunction(overrideName: "BufferData", docs: "https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferData2Delegate(int target, nint size, in byte data, int usage);

	[OpenGLLoadedFunction(overrideName: "BufferData", docs: "https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferData3Delegate(int target, nint size, in float data, int usage);

	[OpenGLLoadedFunction(overrideName: "BufferData", docs: "https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferData4Delegate(int target, nint size, in uint data, int usage);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCreateProgram.xhtml")]
	private delegate uint CreateProgramDelegate();

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCreateShader.xhtml")]
	private delegate uint CreateShaderDelegate(int shaderType);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCompileShader.xhtml")]
	private delegate uint CompileShaderDelegate(uint shader);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glDeleteShader.xhtml")]
	private delegate uint DeleteShaderDelegate(uint shader);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glEnableVertexAttribArray.xhtml")]
	private delegate uint EnableVertexAttribArrayDelegate(uint index);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/es3/html/glFramebufferTexture2D.xhtml")]
	private delegate nint FramebufferTexture2DDelegate(int target, int attachment, int textarget, uint texture, int level);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenBuffers.xhtml")]
	private delegate void GenBuffersDelegate(int count, ref uint buffers);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenFramebuffers.xhtml")]
	private delegate void GenFramebuffersDelegate(int n, ref uint ids);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenVertexArrays.xhtml")]
	private delegate void GenVertexArraysDelegate(int n, ref uint arrays);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGetProgramInfoLog.xhtml")]
	private delegate void GetProgramInfoLogDelegate(uint program, int maxLength, ref int length, ref char infoLog);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGetProgram.xhtml")]
	private delegate void GetProgramivDelegate(uint program, int pname, ref int _params);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenFramebuffers.xhtml")]
	private delegate void GetShaderivDelegate(uint shader, int pname, ref int _params);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glLinkProgram.xhtml")]
	private delegate void LinkProgramDelegate(uint program);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glShaderSource.xhtml")]
	private delegate void ShaderSourceDelegate(uint shader, int count, ref nint _string, in int length);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glUseProgram.xhtml")]
	private delegate void UseProgramDelegate(uint program);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribPointer.xhtml")]
	private delegate void VertexAttribPointerDelegate(uint index, int size, int type, bool normalized, int stride, nint pointer);

	[OpenGLLoadedFunction(automaticPrefix: false, docs: "https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_pixel_format.txt")]
	private delegate bool wglChoosePixelFormatARBDelegate(nint hdc, in int piAttribIList, in float pfAttribFList, uint nMaxFormats, ref int piFormats, ref uint nNumFormats);

	[OpenGLLoadedFunction(automaticPrefix: false, docs: "https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_create_context.txt")]
	private delegate nint wglCreateContextAttribsARBDelegate(nint hDC, bool hShareContext, in int attribList);
}
