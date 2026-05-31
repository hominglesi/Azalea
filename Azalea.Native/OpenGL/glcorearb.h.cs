using System.Runtime.InteropServices;

namespace Azalea.Native.OpenGL;

// Core Constants and Functions that were defined after OpenGL 1.0
// They must be dynamically loaded
// Defined here: https://registry.khronos.org/OpenGL/api/GL/glcorearb.h
// To simplify method calls GL_ and gl prefixes are stripped
public static partial class GL
{
	#region Initialization

	public static bool DynamicFunctionsLoaded { get; private set; } = false;

	/// <summary>
	/// A valid OpenGL context must be current before calling this method
	/// </summary>
	public static void LoadDynamicFunctions(Func<string, nint> getProcAddressMethod)
	{
		_bindBuffer = Marshal.GetDelegateForFunctionPointer<BindBufferDelegate>(getProcAddressMethod("glBindBuffer"));
		_bindFramebuffer = Marshal.GetDelegateForFunctionPointer<BindFramebufferDelegate>(getProcAddressMethod("glBindFramebuffer"));
		_bufferData = Marshal.GetDelegateForFunctionPointer<BufferDataDelegate>(getProcAddressMethod("glBufferData"));
		_framebufferTexture2D = Marshal.GetDelegateForFunctionPointer<FramebufferTexture2DDelegate>(getProcAddressMethod("glFramebufferTexture2D"));
		_genBuffers = Marshal.GetDelegateForFunctionPointer<GenBuffersDelegate>(getProcAddressMethod("glGenBuffers"));
		_genFramebuffers = Marshal.GetDelegateForFunctionPointer<GenFramebuffersDelegate>(getProcAddressMethod("glGenFramebuffers"));
		_wglChoosePixelFormatARB = Marshal.GetDelegateForFunctionPointer<wglChoosePixelFormatARBDelegate>(getProcAddressMethod("wglChoosePixelFormatARB"));
		_wglCreateContextAttribsARB = Marshal.GetDelegateForFunctionPointer<wglCreateContextAttribsARBDelegate>(getProcAddressMethod("wglCreateContextAttribsARB"));

		DynamicFunctionsLoaded = true;
	}

	#endregion

	private delegate void BindBufferDelegate(int target, uint buffer);
	private static BindBufferDelegate? _bindBuffer;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml">Official Documentation</see></summary>
	public static void BindBuffer(int target, uint buffer) => _bindBuffer!(target, buffer);

	private delegate bool BindFramebufferDelegate(int target, uint framebuffer);
	private static BindFramebufferDelegate? _bindFramebuffer;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindFramebuffer.xhtml">Official Documentation</see></summary>
	public static bool BindFramebuffer(int target, uint framebuffer)
		=> _bindFramebuffer!(target, framebuffer);

	private delegate void BufferDataDelegate(int target, nint size, in byte data, int usage);
	private static BufferDataDelegate? _bufferData;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBufferData.xhtml">Official Documentation</see></summary>
	public static void BufferData(int target, nint size, in byte data, int usage) => _bufferData!(target, size, in data, usage);

	private delegate nint FramebufferTexture2DDelegate(int target, int attachment, int textarget, uint texture, int level);
	private static FramebufferTexture2DDelegate? _framebufferTexture2D;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/es3/html/glFramebufferTexture2D.xhtml">Official Documentation</see></summary>
	public static nint FramebufferTexture2D(int target, int attachment, int textarget, uint texture, int level)
		=> _framebufferTexture2D!(target, attachment, textarget, texture, level);

	private delegate void GenBuffersDelegate(int count, ref uint buffers);
	private static GenBuffersDelegate? _genBuffers;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenBuffers.xhtml">Official Documentation</see></summary>
	public static void GenBuffers(int count, ref uint buffers)
		=> _genBuffers!(count, ref buffers);

	private delegate bool GenFramebuffersDelegate(int n, ref uint ids);
	private static GenFramebuffersDelegate? _genFramebuffers;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenFramebuffers.xhtml">Official Documentation</see></summary>
	public static bool GenFramebuffers(int n, ref uint ids)
		=> _genFramebuffers!(n, ref ids);

	private delegate bool wglChoosePixelFormatARBDelegate(nint hdc, in int piAttribIList, in float pfAttribFList, uint nMaxFormats, ref int piFormats, ref uint nNumFormats);
	private static wglChoosePixelFormatARBDelegate? _wglChoosePixelFormatARB;
	/// <summary><see href="https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_pixel_format.txt">Official Documentation</see></summary>
	public static bool wglChoosePixelFormatARB(nint hdc, in int piAttribIList, in float pfAttribFList, uint nMaxFormats, ref int piFormats, ref uint nNumFormats)
		=> _wglChoosePixelFormatARB!(hdc, in piAttribIList, in pfAttribFList, nMaxFormats, ref piFormats, ref nNumFormats);

	private delegate nint wglCreateContextAttribsARBDelegate(nint hDC, bool hShareContext, in int attribList);
	private static wglCreateContextAttribsARBDelegate? _wglCreateContextAttribsARB;
	/// <summary><see href="https://registry.khronos.org/OpenGL/extensions/ARB/WGL_ARB_create_context.txt">Official Documentation</see></summary>
	public static nint wglCreateContextAttribsARB(nint hDC, bool hShareContext, in int attribList)
		=> _wglCreateContextAttribsARB!(hDC, hShareContext, in attribList);
}
