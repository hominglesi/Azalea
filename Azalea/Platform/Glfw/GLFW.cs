﻿using Azalea.Utils;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Glfw;

internal static unsafe partial class GLFW
{
	public const string LibraryPath = "glfw3.dll";

	#region Initialization

	[DllImport(LibraryPath, EntryPoint = "glfwInit")]
	private static extern int init();

	public static bool Initialized { get; private set; }

	public static bool Init()
	{
		var success = init() == 1;

		if (success) Initialized = true;

		return success;
	}

	[DllImport(LibraryPath, EntryPoint = "glfwTerminate")]
	public static extern void Terminate();

	#endregion

	#region Input

	[DllImport(LibraryPath, EntryPoint = "glfwSetKeyCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(KeyCallback))]
	public static extern KeyCallback SetKeyCallback(Window window, KeyCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void KeyCallback(Window window, int key, int scancode, int action, int mods);

	[DllImport(LibraryPath, EntryPoint = "glfwSetCharCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(CharCallback))]
	public static extern CharCallback SetCharCallback(Window window, CharCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void CharCallback(Window window, uint charCode);

	[DllImport(LibraryPath, EntryPoint = "glfwSetCursorPosCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(PositionCallback))]
	public static extern PositionCallback SetCursorPosCallback(Window window, PositionCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void PositionCallback(Window window, double x, double y);

	[DllImport(LibraryPath, EntryPoint = "glfwSetMouseButtonCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(MouseButtonCallback))]
	public static extern MouseButtonCallback SetMouseButtonCallback(Window window, MouseButtonCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MouseButtonCallback(Window window, int button, int action, int mods);

	[DllImport(LibraryPath, EntryPoint = "glfwSetScrollCallback", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.FunctionPtr, MarshalTypeRef = typeof(ScrollCallback))]
	public static extern ScrollCallback SetScrollCallback(Window window, ScrollCallback callback);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ScrollCallback(Window window, double xOffset, double yOffset);
	[DllImport(LibraryPath, EntryPoint = "glfwGetCursorPos")]
	private static extern void getCursorPos(Window window, double* xPos, double* yPos);

	public static Vector2 GetCursorPos(Window window)
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

	#region Monitors

	[DllImport(LibraryPath, EntryPoint = "glfwGetPrimaryMonitor")]
	public static extern Monitor GetPrimaryMonitor();

	[DllImport(LibraryPath, EntryPoint = "glfwGetMonitorWorkarea")]
	private static extern Monitor getMonitorWorkarea(Monitor monitor, int* x, int* y, int* width, int* height);

	public static Vector2Int GetMonitorWorkarea(Monitor monitor)
	{
		int x;
		int y;
		int width;
		int height;

		getMonitorWorkarea(monitor, &x, &y, &width, &height);

		return new Vector2Int(width, height);
	}

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowMonitor")]
	public static extern void SetWindowMonitor(Window window, Monitor monitor, int x, int y, int width, int height, int refreshRate);

	[DllImport(LibraryPath, EntryPoint = "glfwSetWindowMonitor")]
	public static extern void SetWindowMonitor(Window window, IntPtr nullPointer, int x, int y, int width, int height, int refreshRate);

	[DllImport(LibraryPath, EntryPoint = "glfwGetVideoMode")]
	private static extern IntPtr getVideoMode(Monitor monitor);

	public static VideoMode GetVideoMode(Monitor monitor)
	{
		var ptr = getVideoMode(monitor);
		return Marshal.PtrToStructure<VideoMode>(ptr);
	}

	#endregion

	#region Misc

	[DllImport(LibraryPath, EntryPoint = "glfwPollEvents")]
	public static extern void PollEvents();

	#endregion
}