using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Desktop.Glfw.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct GLFWImage
{
	public readonly int Width;
	public readonly int Height;
	public readonly IntPtr Pixels;

	public GLFWImage(int width, int height, IntPtr pixels)
	{
		Width = width;
		Height = height;
		Pixels = pixels;
	}
}
