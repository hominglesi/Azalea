using System;

namespace Azalea.Platform.Glfw;
internal readonly struct Window
{
	public readonly IntPtr NativePointer;

	public Window(IntPtr pointer)
	{
		NativePointer = pointer;
	}

	public static implicit operator IntPtr(Window window) => window.NativePointer;
	public static implicit operator Window(IntPtr pointer) => new(pointer);
}
