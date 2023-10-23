using System;

namespace Azalea.Graphics.GLFW;
public readonly struct GLFW_Monitor
{
	public readonly IntPtr NativePointer;

	public GLFW_Monitor(IntPtr pointer)
	{
		NativePointer = pointer;
	}

	public static implicit operator IntPtr(GLFW_Monitor monitor) => monitor.NativePointer;
	public static implicit operator GLFW_Monitor(IntPtr pointer) => new(pointer);
}
