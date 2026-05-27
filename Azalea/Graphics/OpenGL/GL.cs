using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Native.Windows;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Azalea.Graphics.OpenGL;
internal static unsafe class GL
{
	private const string LibraryPath = "opengl32.dll";

	[DllImport(LibraryPath, EntryPoint = "wglDeleteContext")]
	public static extern bool DeleteContext(IntPtr context);

	private delegate void SwapIntervalDelegate(int interval);
	private static SwapIntervalDelegate? _wglSwapInterval;
	public static void SwapInterval(int interval) => _wglSwapInterval!(interval);

	private delegate int GetSwapIntervalDelegate();
	private static GetSwapIntervalDelegate? _wglGetSwapInterval;
	public static int GetSwapInterval() => _wglGetSwapInterval!();

	[DllImport(LibraryPath, EntryPoint = "glGetString")]
	private static extern IntPtr getString(GLStringName name);
	public static string GetString(GLStringName name)
	{
		return Marshal.PtrToStringAnsi(getString(name)) ?? "";
	}

	[DllImport(LibraryPath, EntryPoint = "glGetError")]
	public static extern GLError GetError();

	public static IEnumerable<GLError> GetErrors()
	{
		GLError error;
		while ((error = GetError()) != GLError.None)
		{
			yield return error;
		}
	}

	public static void PrintErrors()
	{
		foreach (var error in GetErrors())
		{
			Console.WriteLine(error);
		}
	}

	[DllImport(LibraryPath, EntryPoint = "glViewport")]
	public static extern void Viewport(int x, int y, int width, int height);

	[DllImport(LibraryPath, EntryPoint = "glBegin")]
	public static extern void Begin(GLBeginMode mode);

	[DllImport(LibraryPath, EntryPoint = "glEnd")]
	public static extern void End();

	[DllImport(LibraryPath, EntryPoint = "glDrawArrays")]
	public static extern void DrawArrays(GLBeginMode mode, int first, int count);

	[DllImport(LibraryPath, EntryPoint = "glDrawElements")]
	private static extern void drawElements(GLBeginMode mode, int size, GLDataType type, void* indices);

	public static void DrawElements(GLBeginMode mode, int size, GLDataType type, int offset)
	{
		drawElements(mode, size, type, ((IntPtr)offset).ToPointer());
	}

	[DllImport(LibraryPath, EntryPoint = "glBlendFunc")]
	public static extern void BlendFunc(GLBlendFunction source, GLBlendFunction destination);

	[DllImport(LibraryPath, EntryPoint = "glScissor")]
	public static extern void Scissor(int x, int y, int width, int height);

	[DllImport(LibraryPath, EntryPoint = "glEnable")]
	public static extern void Enable(GLCapability capability);

	[DllImport(LibraryPath, EntryPoint = "glDisable")]
	public static extern void Disable(GLCapability capability);

	#region Textures

	[DllImport(LibraryPath, EntryPoint = "glDeleteTextures")]
	private static extern void deleteTextures(int size, uint* textures);
	public static void DeleteTexture(uint texture)
	{
		deleteTextures(1, &texture);
	}

	[DllImport(LibraryPath, EntryPoint = "glTexParameteri")]
	public static extern void TexParameteri(GLTextureType type, GLTextureParameter name, int value);

	private delegate void GLTextureSlotDelegate(GLTextureSlot slot);
	private static GLTextureSlotDelegate? _glActiveTexture;
	public static void ActiveTexture(uint slot) => _glActiveTexture!(GLTextureSlot.Texture0 + (int)slot);

	private delegate void GLTextureTypeDelegate(GLTextureType slot);
	private static GLTextureTypeDelegate? _glGenerateMipmap;
	public static void GenerateMipmap(GLTextureType type) => _glGenerateMipmap!(type);

	#endregion

	#region GLVertex

	[DllImport(LibraryPath, EntryPoint = "glVertex2f")]
	public static extern void Vertex2f(float x, float y);

	#endregion

	#region Modern

	private delegate void VoidDelegate();
	private delegate void VoidUIntDelegate(uint value);
	private delegate uint UIntDelegate();

	private delegate void CreateBuffersDelegate(int n, uint* buffers);
	private static CreateBuffersDelegate? _glCreateBuffers;
	public static void CreateBuffers(int n, uint* buffers) => _glCreateBuffers!(n, buffers);


	private delegate void GenBuffersDelegate(int count, uint* buffers);
	private static GenBuffersDelegate? _glGenBuffers;
	public static uint GenBuffer()
	{
		uint buffer;
		_glGenBuffers!(1, &buffer);
		return buffer;
	}

	private delegate void GenVertexArraysDelegate(int count, uint* arrays);
	private static GenVertexArraysDelegate? _glGenVertexArrays;
	public static uint GenVertexArray()
	{
		uint vao;
		_glGenVertexArrays!(1, &vao);
		return vao;
	}

	private delegate void BindBufferDelegate(GLBufferType type, uint buffer);
	private static BindBufferDelegate? _glBindBuffer;
	public static void BindBuffer(GLBufferType type, uint buffer) => _glBindBuffer!(type, buffer);

	private delegate void BindVertexArrayDelegate(uint vertexArray);
	private static BindVertexArrayDelegate? _glBindVertexArray;
	public static void BindVertexArray(uint buffer) => _glBindVertexArray!(buffer);


	private delegate void BufferDataDelegate(GLBufferType type, IntPtr size, void* data, GLUsageHint hint);
	private static BufferDataDelegate? _glBufferData;
	public static void BufferData(GLBufferType type, IntPtr size, void* data, GLUsageHint hint) => _glBufferData!(type, size, data, hint);
	public static void BufferData<T>(GLBufferType type, T[] data, GLUsageHint hint)
		where T : unmanaged
		=> BufferData(type, data, data.Length, hint);

	public static void BufferData<T>(GLBufferType type, T[] data, int size, GLUsageHint hint)
		where T : unmanaged
	{
		fixed (void* ptr = &data[0])
			BufferData(type, new IntPtr(size * sizeof(T)), ptr, hint);
	}

	private delegate void VertexAttribPointerDelegate(uint index, int size, GLDataType type, bool normalized, int stride, void* pointer);
	private static VertexAttribPointerDelegate? _glVertexAttribPointer;
	public static void VertexAttribPointer(uint index, int size, GLDataType type, bool normalized, int stride, int offset)
	{
		_glVertexAttribPointer!(index, size, type, normalized, stride, ((IntPtr)offset).ToPointer());
	}

	public static void VertexAttribPointer(uint index, GLVertexBufferElement element, int stride, int offset)
		=> VertexAttribPointer(index, element.Count, element.Type, element.Normalized, stride, offset);


	private static VoidUIntDelegate? _glEnableVertexAttribArray;
	public static void EnableVertexAttribArray(uint index) => _glEnableVertexAttribArray!(index);


	private static UIntDelegate? _glCreateProgram;
	public static uint CreateProgram() => _glCreateProgram!();


	private delegate uint CreateShaderDelegate(GLShaderType type);
	private static CreateShaderDelegate? _glCreateShader;
	public static uint CreateShader(GLShaderType type) => _glCreateShader!(type);


	private delegate uint ShaderSourceDelegate(uint shader, int count, byte** str, int* length);
	private static ShaderSourceDelegate? _glShaderSource;
	public static void ShaderSource(uint shader, string source)
	{
		var buffer = Encoding.UTF8.GetBytes(source);
		fixed (byte* p1 = &buffer[0])
		{
			var sources = new[] { p1 };
			fixed (byte** p2 = &sources[0])
			{
				var length = buffer.Length;
				_glShaderSource!(shader, 1, p2, &length);
			}
		}
	}

	private static VoidUIntDelegate? _glCompileShader;
	public static void CompileShader(uint shader) => _glCompileShader!(shader);


	private delegate void AttachShaderDelegate(uint program, uint shader);
	private static AttachShaderDelegate? _glAttachShader;
	public static void AttachShader(uint program, uint shader) => _glAttachShader!(program, shader);


	private static VoidUIntDelegate? _glLinkProgram;
	public static void LinkProgram(uint program) => _glLinkProgram!(program);


	private static VoidUIntDelegate? _glValidateProgram;
	public static void ValidateProgram(uint program) => _glValidateProgram!(program);


	private static VoidUIntDelegate? _glDeleteShader;
	public static void DeleteShader(uint shader) => _glDeleteShader!(shader);


	private static VoidUIntDelegate? _glDeleteProgram;
	public static void DeleteProgram(uint program)
	{
		if (BoundProgram == program) UseProgram(0);
		_glDeleteProgram!(program);
	}

	private delegate void GetShaderivDelegate(uint shader, GLParameterName name, int* args);
	private static GetShaderivDelegate? _glGetShaderiv;
	public static void GetShaderiv(uint shader, GLParameterName name, int* args) => _glGetShaderiv!(shader, name, args);

	private delegate void GetShaderInfoLogDelegate(uint shader, int maxLength, out int length, StringBuilder infoLog);
	private static GetShaderInfoLogDelegate? _glGetShaderInfoLog;
	public static void GetShaderInfoLog(uint shader, int maxLength, out int length, StringBuilder infoLog) => _glGetShaderInfoLog!(shader, maxLength, out length, infoLog);

	private delegate void GetProgramivDelegate(uint program, GLParameterName name, int* args);
	private static GetProgramivDelegate? _glGetProgramiv;
	public static void GetProgramiv(uint program, GLParameterName name, int* args) => _glGetProgramiv!(program, name, args);

	public static uint BoundProgram;
	private static VoidUIntDelegate? _glUseProgram;
	public static void UseProgram(uint program)
	{
		if (BoundProgram == program) return;

		_glUseProgram!(program);
		BoundProgram = program;
	}

	private delegate int GetUniformLocationDelegate(uint program, byte* name);
	private static GetUniformLocationDelegate? _glGetUniformLocation;
	public static int GetUniformLocation(uint program, string name)
	{
		var bytes = Encoding.UTF8.GetBytes(name);
		fixed (byte* ptr = &bytes[0])
			return _glGetUniformLocation!(program, ptr);
	}

	private delegate void Uniform1iDelegate(int location, int v);
	private static Uniform1iDelegate? _glUniform1i;
	public static void Uniform1i(int location, int v)
	{
		_glUniform1i!(location, v);
	}

	private delegate void Uniform1ivDelegate(int location, int count, int* v);
	private static Uniform1ivDelegate? _glUniform1iv;
	public static void Uniform1iv(int location, int[] array)
	{
		fixed (int* p = array)
			_glUniform1iv!(location, array.Length, p);
	}

	private delegate void Uniform1fDelegate(int location, float v);
	private static Uniform1fDelegate? _glUniform1f;
	public static void Uniform1f(int location, float v)
	{
		_glUniform1f!(location, v);
	}

	private delegate void Uniform2fDelegate(int location, float v0, float v1);
	private static Uniform2fDelegate? _glUniform2f;
	public static void Uniform2f(int location, float v0, float v1)
	{
		_glUniform2f!(location, v0, v1);
	}

	private delegate void Uniform4fDelegate(int location, float v0, float v1, float v2, float v3);
	private static Uniform4fDelegate? _glUniform4f;
	public static void Uniform4f(int location, float v0, float v1, float v2, float v3)
	{
		_glUniform4f!(location, v0, v1, v2, v3);
	}

	public static void UniformColor(int location, Color color)
	{
		_glUniform4f!(location, color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
	}

	private delegate void UniformMatrix4fvDelegate(int location, int count, GLBool transpose, float* matrix);
	private static UniformMatrix4fvDelegate? _glUniformMatrix4fv;
	public static void UniformMatrix4(int location, int count, bool transpose, Matrix4x4 matrix)
	{
		_glUniformMatrix4fv!(location, count, transpose ? GLBool.True : GLBool.False, (float*)&matrix);
	}


	private delegate void DeleteBuffersDelegate(int size, uint* buffers);
	private static DeleteBuffersDelegate? _glDeleteBuffers;
	public static void DeleteBuffer(uint buffer)
	{
		_glDeleteBuffers!(1, &buffer);
	}

	private delegate void DeleteVertexArrays(int size, uint* array);
	private static DeleteVertexArrays? _glDeleteVertexArrays;
	public static void DeleteVertexArray(uint vertexArray)
	{
		_glDeleteVertexArrays!(1, &vertexArray);
	}

	private delegate bool ChoosePixelFormatARBDelegate(IntPtr deviceContext, ref int attributeIntList, IntPtr attributeFloatList, uint maxFormats, [In, Out] ref int formats, [In, Out] ref uint formatCount);
	private static ChoosePixelFormatARBDelegate? _wglChoosePixelFormatARB;
	public static bool ChoosePixelFormatARB(IntPtr deviceContext, ref int attributeIntList, IntPtr attributeFloatList, uint maxFormats, [In, Out] ref int formats, [In, Out] ref uint formatCount)
	{
		return _wglChoosePixelFormatARB!(deviceContext, ref attributeIntList, attributeFloatList, maxFormats, ref formats, ref formatCount);
	}

	private delegate IntPtr CreateContextAttribsARBDelegate(IntPtr deviceContext, bool shareContext, [In] ref int attributeList);
	private static CreateContextAttribsARBDelegate? _wglCreateContextAttribsARB;
	public static IntPtr CreateContextAttribsARB(IntPtr deviceContext, bool shareContext, [In] ref int attributeList)
	{
		return _wglCreateContextAttribsARB!(deviceContext, shareContext, ref attributeList);
	}

	public static void ImportFunctions()
	{
		_wglSwapInterval = Marshal.GetDelegateForFunctionPointer<SwapIntervalDelegate>(Win32.wglGetProcAddress("wglSwapIntervalEXT"));
		_wglGetSwapInterval = Marshal.GetDelegateForFunctionPointer<GetSwapIntervalDelegate>(Win32.wglGetProcAddress("wglGetSwapIntervalEXT"));
		_glCreateBuffers = Marshal.GetDelegateForFunctionPointer<CreateBuffersDelegate>(Win32.wglGetProcAddress("glCreateBuffers"));
		_glGenBuffers = Marshal.GetDelegateForFunctionPointer<GenBuffersDelegate>(Win32.wglGetProcAddress("glGenBuffers"));
		_glGenVertexArrays = Marshal.GetDelegateForFunctionPointer<GenVertexArraysDelegate>(Win32.wglGetProcAddress("glGenVertexArrays"));
		_glBindBuffer = Marshal.GetDelegateForFunctionPointer<BindBufferDelegate>(Win32.wglGetProcAddress("glBindBuffer"));
		_glBindVertexArray = Marshal.GetDelegateForFunctionPointer<BindVertexArrayDelegate>(Win32.wglGetProcAddress("glBindVertexArray"));
		_glBufferData = Marshal.GetDelegateForFunctionPointer<BufferDataDelegate>(Win32.wglGetProcAddress("glBufferData"));
		_glVertexAttribPointer = Marshal.GetDelegateForFunctionPointer<VertexAttribPointerDelegate>(Win32.wglGetProcAddress("glVertexAttribPointer"));
		_glEnableVertexAttribArray = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(Win32.wglGetProcAddress("glEnableVertexAttribArray"));
		_glCreateProgram = Marshal.GetDelegateForFunctionPointer<UIntDelegate>(Win32.wglGetProcAddress("glCreateProgram"));
		_glCreateShader = Marshal.GetDelegateForFunctionPointer<CreateShaderDelegate>(Win32.wglGetProcAddress("glCreateShader"));
		_glShaderSource = Marshal.GetDelegateForFunctionPointer<ShaderSourceDelegate>(Win32.wglGetProcAddress("glShaderSource"));
		_glCompileShader = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(Win32.wglGetProcAddress("glCompileShader"));
		_glAttachShader = Marshal.GetDelegateForFunctionPointer<AttachShaderDelegate>(Win32.wglGetProcAddress("glAttachShader"));
		_glLinkProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(Win32.wglGetProcAddress("glLinkProgram"));
		_glValidateProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(Win32.wglGetProcAddress("glValidateProgram"));
		_glDeleteShader = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(Win32.wglGetProcAddress("glDeleteShader"));
		_glDeleteProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(Win32.wglGetProcAddress("glDeleteProgram"));
		_glGetShaderiv = Marshal.GetDelegateForFunctionPointer<GetShaderivDelegate>(Win32.wglGetProcAddress("glGetShaderiv"));
		_glGetShaderInfoLog = Marshal.GetDelegateForFunctionPointer<GetShaderInfoLogDelegate>(Win32.wglGetProcAddress("glGetShaderInfoLog"));
		_glGetProgramiv = Marshal.GetDelegateForFunctionPointer<GetProgramivDelegate>(Win32.wglGetProcAddress("glGetProgramiv"));
		_glUseProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(Win32.wglGetProcAddress("glUseProgram"));
		_glGetUniformLocation = Marshal.GetDelegateForFunctionPointer<GetUniformLocationDelegate>(Win32.wglGetProcAddress("glGetUniformLocation"));
		_glUniform1i = Marshal.GetDelegateForFunctionPointer<Uniform1iDelegate>(Win32.wglGetProcAddress("glUniform1i"));
		_glUniform1iv = Marshal.GetDelegateForFunctionPointer<Uniform1ivDelegate>(Win32.wglGetProcAddress("glUniform1iv"));
		_glUniform1f = Marshal.GetDelegateForFunctionPointer<Uniform1fDelegate>(Win32.wglGetProcAddress("glUniform1f"));
		_glUniform2f = Marshal.GetDelegateForFunctionPointer<Uniform2fDelegate>(Win32.wglGetProcAddress("glUniform2f"));
		_glUniform4f = Marshal.GetDelegateForFunctionPointer<Uniform4fDelegate>(Win32.wglGetProcAddress("glUniform4f"));
		_glUniformMatrix4fv = Marshal.GetDelegateForFunctionPointer<UniformMatrix4fvDelegate>(Win32.wglGetProcAddress("glUniformMatrix4fv"));
		_glDeleteBuffers = Marshal.GetDelegateForFunctionPointer<DeleteBuffersDelegate>(Win32.wglGetProcAddress("glDeleteBuffers"));
		_glDeleteVertexArrays = Marshal.GetDelegateForFunctionPointer<DeleteVertexArrays>(Win32.wglGetProcAddress("glDeleteVertexArrays"));
		_glActiveTexture = Marshal.GetDelegateForFunctionPointer<GLTextureSlotDelegate>(Win32.wglGetProcAddress("glActiveTexture"));
		_glGenerateMipmap = Marshal.GetDelegateForFunctionPointer<GLTextureTypeDelegate>(Win32.wglGetProcAddress("glGenerateMipmap"));
		_wglChoosePixelFormatARB = Marshal.GetDelegateForFunctionPointer<ChoosePixelFormatARBDelegate>(Win32.wglGetProcAddress("wglChoosePixelFormatARB"));
		_wglCreateContextAttribsARB = Marshal.GetDelegateForFunctionPointer<CreateContextAttribsARBDelegate>(Win32.wglGetProcAddress("wglCreateContextAttribsARB"));
	}

	#endregion
}
