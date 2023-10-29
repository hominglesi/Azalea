using Azalea.Graphics.GLFW.Enums;
using Azalea.Utils;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Azalea.Graphics.GLFW;
internal static unsafe class GLFW
{
	public const string LibraryPath = "glfw3.dll";

	#region Initialization

	[DllImport(LibraryPath, EntryPoint = "glfwInit")]
	private static extern int init();

	public static bool Init()
	{
		return init() == 1;
	}

	[DllImport(LibraryPath, EntryPoint = "glfwTerminate")]
	public static extern void Terminate();

	#endregion

	#region Window

	[DllImport(LibraryPath, EntryPoint = "glfwCreateWindow")]
	private static extern GLFW_Window createWindow(int width, int height, byte[] title, GLFW_Monitor monitor, GLFW_Window share);

	public static GLFW_Window CreateWindow(int width, int height, string title, GLFW_Monitor? monitor, GLFW_Window? share)
	{
		return createWindow(width, height, Encoding.UTF8.GetBytes(title), monitor ?? IntPtr.Zero, share ?? IntPtr.Zero);
	}

	[DllImport(LibraryPath, EntryPoint = "glfwWindowShouldClose")]
	private static extern int windowShouldClose(GLFW_Window window);

	public static bool WindowShouldClose(GLFW_Window window)
	{
		return windowShouldClose(window) == 1;
	}

	[DllImport(LibraryPath, EntryPoint = "glfwWindowHint")]
	public static extern void WindowHint(GLFWWindowHint hint, int value);

	public static void OpenGLProfileHint(GLFWOpenGLProfile profile) => WindowHint(GLFWWindowHint.OpenGLProfile, (int)profile);

	[DllImport(LibraryPath, EntryPoint = "glfwMakeContextCurrent")]
	public static extern void MakeContextCurrent(GLFW_Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSwapBuffers")]
	public static extern void SwapBuffers(GLFW_Window window);

	#endregion

	#region Events

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowSizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(SizeCallback))]
	public static extern SizeCallback SetWindowSizeCallback(GLFW_Window window, SizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SizeCallback(GLFW_Window window, int width, int height);

	[DllImport(LibraryPath, EntryPoint = "glfwSetFramebufferSizeCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(FramebufferSizeCallback))]
	public static extern FramebufferSizeCallback SetFramebufferSizeCallback(GLFW_Window window, FramebufferSizeCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void FramebufferSizeCallback(GLFW_Window window, int width, int height);

	#endregion

	#region Input

	[DllImport(LibraryPath, EntryPoint = "glfwSetKeyCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(KeyCallback))]
	public static extern KeyCallback SetKeyCallback(GLFW_Window window, KeyCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void KeyCallback(GLFW_Window window, int key, int scancode, int action, int mods);

	[DllImport(LibraryPath, EntryPoint = "glfwSetCursorPosCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(PositionCallback))]
	public static extern PositionCallback SetCursorPosCallback(GLFW_Window window, PositionCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void PositionCallback(GLFW_Window window, double x, double y);

	[DllImport(LibraryPath, EntryPoint = "glfwSetMouseButtonCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(MouseButtonCallback))]
	public static extern MouseButtonCallback SetMouseButtonCallback(GLFW_Window window, MouseButtonCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MouseButtonCallback(GLFW_Window window, int button, int action, int mods);

	[DllImport(LibraryPath, EntryPoint = "glfwSetScrollCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(ScrollCallback))]
	public static extern ScrollCallback SetScrollCallback(GLFW_Window window, ScrollCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ScrollCallback(GLFW_Window window, double xOffset, double yOffset);
	[DllImport(LibraryPath, EntryPoint = "glfwGetCursorPos")]
	private static extern void getCursorPos(GLFW_Window window, double* xPos, double* yPos);

	public static Vector2 GetCursorPos(GLFW_Window window)
	{
		double xPos = 0;
		double yPos = 0;

		getCursorPos(window, &xPos, &yPos);

		return new Vector2((float)xPos, (float)yPos);
	}

	[DllImport(LibraryPath, EntryPoint = "glfwJoystickPresent")]
	private static extern int joystickPresent(int joystickId);

	public static bool JoystickPresent(int joystickId)
		=> joystickPresent(joystickId) == 1;


	[DllImport(LibraryPath, EntryPoint = "glfwGetJoystickName")]
	private static extern char* getJoystickName(int joystickId);
	public static string GetJoystickName(int joystickId)
		=> Marshal.PtrToStringUTF8((IntPtr)getJoystickName(joystickId)) ?? "";

	[DllImport(LibraryPath, EntryPoint = "glfwGetJoystickAxes")]
	private static extern float* getJoystickAxes(int joystickId, int* count);
	public static Vector2[] GetJoystickAxes(int joystickId)
	{
		int count;
		var data = getJoystickAxes(joystickId, &count);
		var axes = new Vector2[count / 2];

		for (int i = 0; i + 1 < count; i += 2)
		{
			var j = i + 1;
			var x = data[i] > Precision.FLOAT_EPSILON || data[i] < -Precision.FLOAT_EPSILON ? data[i] : 0;
			var y = data[j] > Precision.FLOAT_EPSILON || data[j] < -Precision.FLOAT_EPSILON ? data[j] : 0;

			axes[i / 2] = new Vector2(x, y);
		}

		return axes;
	}

	#endregion

	#region Misc

	[DllImport(LibraryPath, EntryPoint = "glfwPollEvents")]
	public static extern void PollEvents();

	#endregion
}
