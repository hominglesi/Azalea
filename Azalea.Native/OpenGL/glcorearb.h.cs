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
		_bindFramebuffer = Marshal.GetDelegateForFunctionPointer<BindFramebufferDelegate>(getProcAddressMethod("glBindFramebuffer"));
		_framebufferTexture2D = Marshal.GetDelegateForFunctionPointer<FramebufferTexture2DDelegate>(getProcAddressMethod("glFramebufferTexture2D"));
		_genFramebuffers = Marshal.GetDelegateForFunctionPointer<GenFramebuffersDelegate>(getProcAddressMethod("glGenFramebuffers"));
		_wglChoosePixelFormatARB = Marshal.GetDelegateForFunctionPointer<wglChoosePixelFormatARBDelegate>(getProcAddressMethod("wglChoosePixelFormatARB"));
		_wglCreateContextAttribsARB = Marshal.GetDelegateForFunctionPointer<wglCreateContextAttribsARBDelegate>(getProcAddressMethod("wglCreateContextAttribsARB"));

		DynamicFunctionsLoaded = true;
	}

	#endregion

	private delegate bool BindFramebufferDelegate(int target, uint framebuffer);
	private static BindFramebufferDelegate? _bindFramebuffer;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindFramebuffer.xhtml">Official Documentation</see></summary>
	public static bool BindFramebuffer(int target, uint framebuffer)
		=> _bindFramebuffer!(target, framebuffer);

	private delegate nint FramebufferTexture2DDelegate(int target, int attachment, int textarget, uint texture, int level);
	private static FramebufferTexture2DDelegate? _framebufferTexture2D;
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/es3/html/glFramebufferTexture2D.xhtml">Official Documentation</see></summary>
	public static nint FramebufferTexture2D(int target, int attachment, int textarget, uint texture, int level)
		=> _framebufferTexture2D!(target, attachment, textarget, texture, level);

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
