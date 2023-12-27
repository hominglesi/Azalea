using Azalea.Graphics;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Azalea.Platform.Glfw;
internal static unsafe partial class GLFW
{
	#region Creation

	[DllImport(LibraryPath, EntryPoint = "glfwCreateWindow")]
	private static extern Window createWindow(int width, int height, byte[] title, Monitor monitor, Window share);

	public static Window CreateWindow(int width, int height, string title, Monitor? monitor, Window? share)
		=> createWindow(width, height, Encoding.UTF8.GetBytes(title), monitor ?? IntPtr.Zero, share ?? IntPtr.Zero);

	public static Window CreateWindow(Vector2Int size, string title, Monitor? monitor, Window? window)
		=> CreateWindow(size.X, size.Y, title, monitor, window);

	#endregion
	#region Hints & Attributes

	//Hints
	[DllImport(LibraryPath, EntryPoint = "glfwWindowHint")]
	public static extern void WindowHint(WindowHint hint, int value);
	public static void WindowHint(WindowHint hint, bool value)
		=> WindowHint(hint, value ? 1 : 0);

	//Attributes
	[DllImport(LibraryPath, EntryPoint = "glfwGetWindowAttrib")]
	public static extern int GetWindowAttribute(Window window, WindowAttribute attribute);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowAttrib")]
	public static extern void SetWindowAttribute(Window window, WindowAttribute attribute, int value);
	public static void SetWindowAttribute(Window window, WindowAttribute attribute, bool value)
		=> SetWindowAttribute(window, attribute, value ? 1 : 0);

	#endregion
	#region Icon

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowIcon")]
	private static extern void setWindowIcon(Window window, int count, Image image);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowIcon")]
	private static extern void setWindowIcon(Window window, int count, IntPtr nullPointer);

	public static void SetWindowIcon(Window window, Graphics.Image? data)
	{
		if (data is null)
		{
			setWindowIcon(window, 0, IntPtr.Zero);
			return;
		}

		fixed (byte* p = data.Data)
		{
			var image = new Image(data.Width, data.Height, (IntPtr)p);
			setWindowIcon(window, 1, image);
		}
	}

	#endregion
	#region Visibility

	[DllImport(LibraryPath, EntryPoint = "glfwShowWindow")]
	public static extern void ShowWindow(Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwHideWindow")]
	public static extern void HideWindow(Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowOpacity")]
	public static extern void SetWindowOpacity(Window window, float opacity);

	#endregion
	#region Focus

	[DllImport(LibraryPath, EntryPoint = "glfwFocusWindow")]
	public static extern void FocusWindow(Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwRequestWindowAttention")]
	public static extern void RequestWindowAttention(Window window);

	#endregion
	#region States

	[DllImport(LibraryPath, EntryPoint = "glfwRestoreWindow")]
	public static extern void RestoreWindow(Window window);

	//Minimize
	[DllImport(LibraryPath, EntryPoint = "glfwIconifyWindow")]
	public static extern void IconifyWindow(Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowIconifyCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowIconifyCallback))]
	public static extern WindowIconifyCallback SetWindowIconifyCallback(Window window, WindowIconifyCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowIconifyCallback(Window window, int iconified);

	//Maximize
	[DllImport(LibraryPath, EntryPoint = "glfwMaximizeWindow")]
	public static extern void MaximizeWindow(Window window);
	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowMaximizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowMaximizeCallback))]
	public static extern WindowMaximizeCallback SetWindowMaximizeCallback(Window window, WindowMaximizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowMaximizeCallback(Window window, int maximized);

	#endregion
	#region GL

	[DllImport(LibraryPath, EntryPoint = "glfwMakeContextCurrent")]
	public static extern void MakeContextCurrent(Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSwapBuffers")]
	public static extern void SwapBuffers(Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSwapInterval")]
	public static extern void SwapInterval(int interval);

	#endregion
	#region Title

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowTitle")]
	public static extern void SetWindowTitle(Window window, string title);

	#endregion
	#region Position

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowPos")]
	public static extern void SetWindowPos(Window window, int x, int y);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowPosCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowPositionCallback))]
	public static extern WindowPositionCallback SetWindowPosCallback(Window window, WindowPositionCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowPositionCallback(Window window, int x, int y);

	#endregion
	#region Size

	//Window Size
	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowSize")]
	public static extern void SetWindowSize(Window window, int width, int height);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowSizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(SizeCallback))]
	public static extern SizeCallback SetWindowSizeCallback(Window window, SizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SizeCallback(Window window, int width, int height);

	//Framebuffer Size
	[DllImport(LibraryPath, EntryPoint = "glfwSetFramebufferSizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(FramebufferSizeCallback))]
	public static extern FramebufferSizeCallback SetFramebufferSizeCallback(Window window, FramebufferSizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void FramebufferSizeCallback(Window window, int width, int height);

	//Window Frame Size
	[DllImport(LibraryPath, EntryPoint = "glfwGetWindowFrameSize")]
	private static extern void getWindowFrameSize(Window window, int* left, int* top, int* right, int* bottom);
	public static Boundary GetWindowFrameSize(Window window)
	{
		int left, top, right, bottom;

		getWindowFrameSize(window, &left, &top, &right, &bottom);

		return new Boundary(top, right, bottom, left);
	}

	#endregion
	#region Closing

	[DllImport(LibraryPath, EntryPoint = "glfwWindowShouldClose")]
	private static extern int windowShouldClose(Window window);

	public static bool WindowShouldClose(Window window)
	{
		return windowShouldClose(window) == 1;
	}

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowShouldClose")]
	private static extern void setWindowShouldClose(Window window, int value);

	public static void SetWindowShouldClose(Window window, bool value)
	{
		setWindowShouldClose(window, value ? 1 : 0);
	}

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowCloseCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowCloseCallback))]
	public static extern WindowCloseCallback SetWindowCloseCallback(Window window, WindowCloseCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowCloseCallback(Window window);

	#endregion
}
