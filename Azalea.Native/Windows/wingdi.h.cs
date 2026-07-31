using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	private const string Gdi32Path = "gdi32.dll";
	private const string OpenGLPath = "opengl32.dll";

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-choosepixelformat">Official Documentation</see></summary>
	[LibraryImport(Gdi32Path)]
	public static partial int ChoosePixelFormat(nint hdc, in PIXELFORMATDESCRIPTOR descriptor);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-createsolidbrush">Official Documentation</see></summary>
	[LibraryImport(Gdi32Path)]
	public static partial nint CreateSolidBrush(uint colorRef);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-describepixelformat">Official Documentation</see></summary>
	[LibraryImport(Gdi32Path)]
	public static partial int DescribePixelFormat(nint hdc, int iPixelFormat, uint nBytes, ref PIXELFORMATDESCRIPTOR ppfd);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-pixelformatdescriptor">Official Documentation</see></summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct PIXELFORMATDESCRIPTOR
	{
		public readonly uint nSize = (uint)Marshal.SizeOf<PIXELFORMATDESCRIPTOR>();
		public uint nVersion = 1;
		[Flags]
		internal enum Flags
		{
			DRAW_TO_WINDOW = 0x00000004,
			DRAW_TO_BITMAP = 0x00000008,
			SUPPORT_GDI = 0x00000010,
			SUPPORT_OPENGL = 0x00000020,
			GENERIC_ACCELERATED = 0x00001000,
			GENERIC_FORMAT = 0x00000040,
			NEED_PALETTE = 0x00000080,
			NEED_SYSTEM_PALETTE = 0x00000100,
			DOUBLEBUFFER = 0x00000001,
			STEREO = 0x00000002,
			SWAP_LAYER_BUFFERS = 0x00000800,
		}
		public int dwFlags = (int)(Flags.DRAW_TO_WINDOW | Flags.SUPPORT_OPENGL | Flags.DOUBLEBUFFER);
		public byte iPixelType;
		public byte cColorBits = 32;
		public byte cRedBits;
		public byte cRedShift;
		public byte cGreenBits;
		public byte cGreenShift;
		public byte cBlueBits;
		public byte cBlueShift;
		public byte cAlphaBits = 8;
		public byte cAlphaShift;
		public byte cAccumBits;
		public byte cAccumRedBits;
		public byte cAccumGreenBits;
		public byte cAccumBlueBits;
		public byte cAccumAlphaBits;
		public byte cDepthBits = 24;
		public byte cStencilBits = 8;
		public byte cAuxBuffers;
		public byte iLayerType;
		public byte bReserved;
		public int dwLayerMask;
		public int dwVisibleMask;
		public int dwDamageMask;

		public PIXELFORMATDESCRIPTOR() { }
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-setpixelformat">Official Documentation</see></summary>
	[LibraryImport(Gdi32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SetPixelFormat(nint hdc, int format, in PIXELFORMATDESCRIPTOR ppfd);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-swapbuffers">Official Documentation</see></summary>
	[LibraryImport(Gdi32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SwapBuffers(nint hdc);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-wglcreatecontext">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath)]
	public static partial nint wglCreateContext(nint handleToDeviceContext);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-wglgetprocaddress">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath, StringMarshalling = StringMarshalling.Utf8)]
	public static partial nint wglGetProcAddress(string functionName);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-wglmakecurrent">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool wglMakeCurrent(nint hdc, nint hglrc);
}
