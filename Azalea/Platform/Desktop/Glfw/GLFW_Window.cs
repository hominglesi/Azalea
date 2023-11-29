using System;

namespace Azalea.Platform.Desktop.Glfw;
internal readonly struct GLFW_Window
{
	public readonly IntPtr NativePointer;

	public GLFW_Window(IntPtr pointer)
	{
		NativePointer = pointer;
	}

	public static implicit operator IntPtr(GLFW_Window window) => window.NativePointer;
	public static implicit operator GLFW_Window(IntPtr pointer) => new(pointer);
}
