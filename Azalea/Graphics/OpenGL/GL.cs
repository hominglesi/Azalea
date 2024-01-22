using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Azalea.Graphics.OpenGL;
internal static unsafe class GL
{
	private const string LibraryPath = "opengl32.dll";

	[DllImport(LibraryPath, EntryPoint = "wglCreateContext")]
	public static extern IntPtr CreateContext(IntPtr deviceContext);

	[DllImport(LibraryPath, EntryPoint = "wglMakeCurrent")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool MakeCurrent(IntPtr deviceContext, IntPtr glContext);

	private delegate void SwapIntervalDelegate(int interval);
	private static SwapIntervalDelegate? _wglSwapInterval;
	public static void SwapInterval(int interval) => _wglSwapInterval!(interval);

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

	[DllImport(LibraryPath, EntryPoint = "glClear")]
	public static extern void Clear(GLBufferBit bufferBits);

	[DllImport(LibraryPath, EntryPoint = "glClearColor")]
	private static extern void clearColor(float red, float green, float blue, float alpha);
	public static void ClearColor(Color color)
	{
		clearColor(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
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

	[DllImport(LibraryPath, EntryPoint = "glBindTexture")]
	public static extern void BindTexture(GLTextureType type, uint texture);

	[DllImport(LibraryPath, EntryPoint = "glGenTextures")]
	private static extern void genTextures(int size, uint* textures);
	public static uint GenTexture()
	{
		uint texture;
		genTextures(1, &texture);
		return texture;
	}

	[DllImport(LibraryPath, EntryPoint = "glDeleteTextures")]
	private static extern void deleteTextures(int size, uint* textures);
	public static void DeleteTexture(uint texture)
	{
		deleteTextures(1, &texture);
	}

	[DllImport(LibraryPath, EntryPoint = "glTexParameteri")]
	public static extern void TexParameteri(GLTextureType type, GLTextureParameter name, int value);

	[DllImport(LibraryPath, EntryPoint = "glTexImage2D")]
	private static extern void texImage2D(GLTextureType type, int level, GLColorFormat internalFormat,
		int width, int height, int border, GLColorFormat format, GLDataType dataType, void* pixels);

	public static void TexImage2D(GLTextureType type, int level, GLColorFormat internalFormat,
		int width, int height, int border, GLColorFormat format, GLDataType dataType, byte[] pixels)
	{
		fixed (void* p = &pixels[0])
			texImage2D(type, level, internalFormat, width, height, border, format, dataType, p);
	}

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

	//Function for getting the modern function pointers
	[DllImport(LibraryPath, EntryPoint = "wglGetProcAddress")]
	public static extern IntPtr wglGetProcAddress(string functionName);
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

	public static void Import()
	{
		_wglSwapInterval = Marshal.GetDelegateForFunctionPointer<SwapIntervalDelegate>(wglGetProcAddress("wglSwapIntervalEXT"));
		_glCreateBuffers = Marshal.GetDelegateForFunctionPointer<CreateBuffersDelegate>(wglGetProcAddress("glCreateBuffers"));
		_glGenBuffers = Marshal.GetDelegateForFunctionPointer<GenBuffersDelegate>(wglGetProcAddress("glGenBuffers"));
		_glGenVertexArrays = Marshal.GetDelegateForFunctionPointer<GenVertexArraysDelegate>(wglGetProcAddress("glGenVertexArrays"));
		_glBindBuffer = Marshal.GetDelegateForFunctionPointer<BindBufferDelegate>(wglGetProcAddress("glBindBuffer"));
		_glBindVertexArray = Marshal.GetDelegateForFunctionPointer<BindVertexArrayDelegate>(wglGetProcAddress("glBindVertexArray"));
		_glBufferData = Marshal.GetDelegateForFunctionPointer<BufferDataDelegate>(wglGetProcAddress("glBufferData"));
		_glVertexAttribPointer = Marshal.GetDelegateForFunctionPointer<VertexAttribPointerDelegate>(wglGetProcAddress("glVertexAttribPointer"));
		_glEnableVertexAttribArray = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(wglGetProcAddress("glEnableVertexAttribArray"));
		_glCreateProgram = Marshal.GetDelegateForFunctionPointer<UIntDelegate>(wglGetProcAddress("glCreateProgram"));
		_glCreateShader = Marshal.GetDelegateForFunctionPointer<CreateShaderDelegate>(wglGetProcAddress("glCreateShader"));
		_glShaderSource = Marshal.GetDelegateForFunctionPointer<ShaderSourceDelegate>(wglGetProcAddress("glShaderSource"));
		_glCompileShader = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(wglGetProcAddress("glCompileShader"));
		_glAttachShader = Marshal.GetDelegateForFunctionPointer<AttachShaderDelegate>(wglGetProcAddress("glAttachShader"));
		_glLinkProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(wglGetProcAddress("glLinkProgram"));
		_glValidateProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(wglGetProcAddress("glValidateProgram"));
		_glDeleteShader = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(wglGetProcAddress("glDeleteShader"));
		_glDeleteProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(wglGetProcAddress("glDeleteProgram"));
		_glGetShaderiv = Marshal.GetDelegateForFunctionPointer<GetShaderivDelegate>(wglGetProcAddress("glGetShaderiv"));
		_glGetProgramiv = Marshal.GetDelegateForFunctionPointer<GetProgramivDelegate>(wglGetProcAddress("glGetProgramiv"));
		_glUseProgram = Marshal.GetDelegateForFunctionPointer<VoidUIntDelegate>(wglGetProcAddress("glUseProgram"));
		_glGetUniformLocation = Marshal.GetDelegateForFunctionPointer<GetUniformLocationDelegate>(wglGetProcAddress("glGetUniformLocation"));
		_glUniform1i = Marshal.GetDelegateForFunctionPointer<Uniform1iDelegate>(wglGetProcAddress("glUniform1i"));
		_glUniform1iv = Marshal.GetDelegateForFunctionPointer<Uniform1ivDelegate>(wglGetProcAddress("glUniform1iv"));
		_glUniform4f = Marshal.GetDelegateForFunctionPointer<Uniform4fDelegate>(wglGetProcAddress("glUniform4f"));
		_glUniformMatrix4fv = Marshal.GetDelegateForFunctionPointer<UniformMatrix4fvDelegate>(wglGetProcAddress("glUniformMatrix4fv"));
		_glDeleteBuffers = Marshal.GetDelegateForFunctionPointer<DeleteBuffersDelegate>(wglGetProcAddress("glDeleteBuffers"));
		_glDeleteVertexArrays = Marshal.GetDelegateForFunctionPointer<DeleteVertexArrays>(wglGetProcAddress("glDeleteVertexArrays"));
		_glActiveTexture = Marshal.GetDelegateForFunctionPointer<GLTextureSlotDelegate>(wglGetProcAddress("glActiveTexture"));
		_glGenerateMipmap = Marshal.GetDelegateForFunctionPointer<GLTextureTypeDelegate>(wglGetProcAddress("glGenerateMipmap"));
	}

	#endregion
}
