using System;

namespace Azalea.Sounds.OpenAL;
internal readonly struct ALC_Device
{
	public readonly IntPtr NativePointer;

	public ALC_Device(IntPtr pointer)
	{
		NativePointer = pointer;
	}

	public static implicit operator IntPtr(ALC_Device device) => device.NativePointer;
	public static implicit operator ALC_Device(IntPtr pointer) => new(pointer);
}
