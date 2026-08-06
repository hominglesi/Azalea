using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

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

	public const int DEBUG_OUTPUT_SYNCHRONOUS = 0x8242;
	public const int DEBUG_OUTPUT = 0x92E0;
	public const int DEBUG_SEVERITY_HIGH = 0x9146;
	public const int DEBUG_SEVERITY_MEDIUM = 0x9147;
	public const int DEBUG_SEVERITY_LOW = 0x9148;
	public const int DEBUG_SEVERITY_NOTIFICATION = 0x826B;

	public const int UNIFORM_BUFFER = 0x8A11;

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

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBufferRange.xhtml")]
	private delegate void BindBufferRangeDelegate(int target, uint index, uint buffer, nint offset, nint size);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindFramebuffer.xhtml")]
	private delegate void BindFramebufferDelegate(int target, uint framebuffer);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindVertexArray.xhtml")]
	private delegate void BindVertexArrayDelegate(uint array);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBlitFramebuffer.xhtml")]
	private delegate void BlitFramebufferDelegate(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, int mask, int filter);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferDataDelegate(int target, nint size, nint data, int usage);

	[OpenGLLoadedFunction(overrideName: "BufferData", docs: "https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferData2Delegate(int target, nint size, in byte data, int usage);

	[OpenGLLoadedFunction(overrideName: "BufferData", docs: "https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferData3Delegate(int target, nint size, in float data, int usage);

	[OpenGLLoadedFunction(overrideName: "BufferData", docs: "https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml")]
	private delegate void BufferData4Delegate(int target, nint size, in uint data, int usage);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferSubData.xhtml")]
	private delegate void BufferSubDataDelegate(int target, nint offset, nint size, nint data);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCreateProgram.xhtml")]
	private delegate uint CreateProgramDelegate();

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCreateShader.xhtml")]
	private delegate uint CreateShaderDelegate(int shaderType);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glCompileShader.xhtml")]
	private delegate uint CompileShaderDelegate(uint shader);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void DebugProc(uint source, uint type, uint id, uint severity, int length, nint message, nint userParam);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glDebugMessageCallback.xhtml")]
	private delegate uint DebugMessageCallbackDelegate(DebugProc callback, nint userParam);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glDeleteShader.xhtml")]
	private delegate uint DeleteShaderDelegate(uint shader);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glEnableVertexAttribArray.xhtml")]
	private delegate uint EnableVertexAttribArrayDelegate(uint index);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/es3/html/glFramebufferTexture2D.xhtml")]
	private delegate nint FramebufferTexture2DDelegate(int target, int attachment, int textarget, uint texture, int level);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenBuffers.xhtml")]
	private delegate void GenBuffersDelegate(int count, ref uint buffers);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenerateMipmap.xhtml")]
	private delegate void GenerateMipmapDelegate(int target);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenFramebuffers.xhtml")]
	private delegate void GenFramebuffersDelegate(int n, ref uint ids);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenVertexArrays.xhtml")]
	private delegate void GenVertexArraysDelegate(int n, ref uint arrays);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGetProgramInfoLog.xhtml")]
	private delegate void GetProgramInfoLogDelegate(uint program, int maxLength, out int length, StringBuilder infoLog);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGetProgram.xhtml")]
	private delegate void GetProgramivDelegate(uint program, int pname, ref int _params);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGetShaderInfoLog.xhtml")]
	private delegate void GetShaderInfoLogDelegate(uint shader, int maxLength, out int length, StringBuilder infoLog);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGetShader.xhtml")]
	private delegate void GetShaderivDelegate(uint shader, int pname, ref int _params);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGetUniformLocation.xhtml")]
	private delegate int GetUniformLocationDelegate(uint program, byte[] name);

	/* Method could not be found on my laptop, disabled for now; Might work with LibraryImport since it's OpenGL 1.1
	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glIsTexture.xhtml")]
	private delegate bool IsTextureDelegate(uint texture);
	*/

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glLinkProgram.xhtml")]
	private delegate void LinkProgramDelegate(uint program);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glShaderSource.xhtml")]
	private delegate void ShaderSourceDelegate(uint shader, int count, ref nint _string, in int length);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glUniform.xhtml")]
	private delegate void Uniform1iDelegate(int location, int v0);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glUniform.xhtml")]
	private delegate void Uniform4fDelegate(int location, float v0, float v1, float v2, float v3);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glUniform.xhtml")]
	private delegate void UniformMatrix4fvDelegate(int location, int count, bool transpose, ref Matrix4x4 value);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glUseProgram.xhtml")]
	private delegate void UseProgramDelegate(uint program);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glValidateProgram.xhtml")]
	private delegate void ValidateProgramDelegate(uint program);

	[OpenGLLoadedFunction("https://registry.khronos.org/OpenGL-Refpages/gl4/html/glVertexAttribPointer.xhtml")]
	private delegate void VertexAttribPointerDelegate(uint index, int size, int type, bool normalized, int stride, nint pointer);

	[OpenGLLoadedFunction(automaticPrefix: false, docs: "https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_pixel_format.txt")]
	private delegate bool wglChoosePixelFormatARBDelegate(nint hdc, in int piAttribIList, in float pfAttribFList, uint nMaxFormats, ref int piFormats, ref uint nNumFormats);

	[OpenGLLoadedFunction(automaticPrefix: false, docs: "https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_create_context.txt")]
	private delegate nint wglCreateContextAttribsARBDelegate(nint hDC, nint hShareContext, in int attribList);

	[OpenGLLoadedFunction(automaticPrefix: false, docs: "https://registry.khronos.org/OpenGL/extensions/EXT/WGL_EXT_swap_control.txt")]
	private delegate bool wglSwapIntervalEXTDelegate(int interval);
}
