﻿using System;
using System.Runtime.InteropServices;

namespace Azalea.Graphics.GLFW;
internal static class GLFW
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
	private static extern GLFW_Window createWindow(int width, int height, char[] title, GLFW_Monitor monitor, GLFW_Window share);

	public static GLFW_Window CreateWindow(int width, int height, string title, GLFW_Monitor? monitor, GLFW_Window? share)
	{
		return createWindow(width, height, title.ToCharArray(), monitor ?? IntPtr.Zero, share ?? IntPtr.Zero);
	}

	[DllImport(LibraryPath, EntryPoint = "glfwWindowShouldClose")]
	private static extern int windowShouldClose(GLFW_Window window);

	public static bool WindowShouldClose(GLFW_Window window)
	{
		return windowShouldClose(window) == 1;
	}

	[DllImport(LibraryPath, EntryPoint = "glfwMakeContextCurrent")]
	public static extern void MakeContextCurrent(GLFW_Window window);

	[DllImport(LibraryPath, EntryPoint = "glfwSwapBuffers")]
	public static extern void SwapBuffers(GLFW_Window window);

	#endregion

	#region Misc

	[DllImport(LibraryPath, EntryPoint = "glfwPollEvents")]
	public static extern void PollEvents();

	#endregion
}