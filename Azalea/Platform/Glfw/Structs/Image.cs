using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Glfw;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct Image
{
	public readonly int Width;
	public readonly int Height;
	public readonly IntPtr Pixels;

	public Image(int width, int height, IntPtr pixels)
	{
		Width = width;
		Height = height;
		Pixels = pixels;
	}
}
