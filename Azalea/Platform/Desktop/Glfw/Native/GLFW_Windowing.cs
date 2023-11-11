using Azalea.Graphics;
using Azalea.Graphics.Textures;
using Azalea.Platform.Desktop.Glfw.Enums;
using Azalea.Platform.Desktop.Glfw.Structs;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Azalea.Platform.Desktop.Glfw.Native;
internal static unsafe partial class GLFW
{
	#region Creation

	[DllImport(LibraryPath, EntryPoint = "glfwCreateWindow")]
	private static extern GLFW_Window createWindow(int width, int height, byte[] title, GLFW_Monitor monitor, GLFW_Window share);

	public static GLFW_Window CreateWindow(int width, int height, string title, GLFW_Monitor? monitor, GLFW_Window? share)
	{
		return createWindow(width, height, Encoding.UTF8.GetBytes(title), monitor ?? IntPtr.Zero, share ?? IntPtr.Zero);
	}

	#endregion
	#region Hints & Attributes

	//Hints
	[DllImport(LibraryPath, EntryPoint = "glfwWindowHint")]
	public static extern void WindowHint(GLFWWindowHint hint, int value);
	public static void WindowHint(GLFWWindowHint hint, bool value)
		=> WindowHint(hint, value ? 1 : 0);

	//Attributes
	[DllImport(LibraryPath, EntryPoint = "glfwGetWindowAttrib")]
	public static extern int GetWindowAttribute(GLFW_Window window, GLFWAttribute attribute);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowAttrib")]
	public static extern void SetWindowAttribute(GLFW_Window window, GLFWAttribute attribute, int value);
	public static void SetWindowAttribute(GLFW_Window window, GLFWAttribute attribute, bool value)
		=> SetWindowAttribute(window, attribute, value ? 1 : 0);

	#endregion
	#region Icon

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowIcon")]
	private static extern void setWindowIcon(GLFW_Window window, int count, GLFWImage image);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowIcon")]
	private static extern void setWindowIcon(GLFW_Window window, int count, IntPtr nullPointer);

	public static void SetWindowIcon(GLFW_Window window, ITextureData? data)
	{
		if (data is null)
		{
			setWindowIcon(window, 0, IntPtr.Zero);
			return;
		}

		fixed (byte* p = data.Data)
		{
			var image = new GLFWImage(data.Width, data.Height, (IntPtr)p);
			setWindowIcon(window, 1, image);
		}
	}

	#endregion
	#region Visibility

	[DllImport(LibraryPath, EntryPoint = "glfwShowWindow")]
	public static extern void ShowWindow(GLFW_Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwHideWindow")]
	public static extern void HideWindow(GLFW_Window window);

	#endregion
	#region Focus

	[DllImport(LibraryPath, EntryPoint = "glfwRequestWindowAttention")]
	public static extern void RequestWindowAttention(GLFW_Window window);

	#endregion
	#region States

	[DllImport(LibraryPath, EntryPoint = "glfwRestoreWindow")]
	public static extern void RestoreWindow(GLFW_Window window);

	//Minimize
	[DllImport(LibraryPath, EntryPoint = "glfwIconifyWindow")]
	public static extern void IconifyWindow(GLFW_Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowIconifyCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowIconifyCallback))]
	public static extern WindowIconifyCallback SetWindowIconifyCallback(GLFW_Window window, WindowIconifyCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowIconifyCallback(GLFW_Window window, int iconified);

	//Maximize
	[DllImport(LibraryPath, EntryPoint = "glfwMaximizeWindow")]
	public static extern void MaximizeWindow(GLFW_Window window);
	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowMaximizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowMaximizeCallback))]
	public static extern WindowMaximizeCallback SetWindowMaximizeCallback(GLFW_Window window, WindowMaximizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowMaximizeCallback(GLFW_Window window, int maximized);

	#endregion
	#region GL

	[DllImport(LibraryPath, EntryPoint = "glfwMakeContextCurrent")]
	public static extern void MakeContextCurrent(GLFW_Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSwapBuffers")]
	public static extern void SwapBuffers(GLFW_Window window);

	#endregion
	#region Title

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowTitle")]
	public static extern void SetWindowTitle(GLFW_Window window, string title);

	#endregion
	#region Position

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowPos")]
	public static extern void SetWindowPos(GLFW_Window window, int x, int y);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowPosCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowPositionCallback))]
	public static extern WindowPositionCallback SetWindowPosCallback(GLFW_Window window, WindowPositionCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowPositionCallback(GLFW_Window window, int x, int y);

	#endregion
	#region Size

	//Window Size
	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowSize")]
	public static extern void SetWindowSize(GLFW_Window window, int width, int height);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowSizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(SizeCallback))]
	public static extern SizeCallback SetWindowSizeCallback(GLFW_Window window, SizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SizeCallback(GLFW_Window window, int width, int height);

	//Framebuffer Size
	[DllImport(LibraryPath, EntryPoint = "glfwSetFramebufferSizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(FramebufferSizeCallback))]
	public static extern FramebufferSizeCallback SetFramebufferSizeCallback(GLFW_Window window, FramebufferSizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void FramebufferSizeCallback(GLFW_Window window, int width, int height);

	//Window Frame Size
	[DllImport(LibraryPath, EntryPoint = "glfwGetWindowFrameSize")]
	private static extern void getWindowFrameSize(GLFW_Window window, int* left, int* top, int* right, int* bottom);
	public static Boundary GetWindowFrameSize(GLFW_Window window)
	{
		int left, top, right, bottom;

		getWindowFrameSize(window, &left, &top, &right, &bottom);

		return new Boundary(top, right, bottom, left);
	}

	#endregion
	#region Closing

	[DllImport(LibraryPath, EntryPoint = "glfwWindowShouldClose")]
	private static extern int windowShouldClose(GLFW_Window window);

	public static bool WindowShouldClose(GLFW_Window window)
	{
		return windowShouldClose(window) == 1;
	}

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowShouldClose")]
	private static extern void setWindowShouldClose(GLFW_Window window, int value);

	public static void SetWindowShouldClose(GLFW_Window window, bool value)
	{
		setWindowShouldClose(window, value ? 1 : 0);
	}

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowCloseCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(WindowCloseCallback))]
	public static extern WindowCloseCallback SetWindowCloseCallback(GLFW_Window window, WindowCloseCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowCloseCallback(GLFW_Window window);

	#endregion
}
