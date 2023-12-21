using System;

namespace Azalea.Platform.Glfw;
internal readonly struct Monitor
{
	public readonly IntPtr NativePointer;

	public Monitor(IntPtr pointer)
	{
		NativePointer = pointer;
	}

	public static implicit operator IntPtr(Monitor monitor) => monitor.NativePointer;
	public static implicit operator Monitor(IntPtr pointer) => new(pointer);
}
