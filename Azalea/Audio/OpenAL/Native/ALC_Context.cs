using System;

namespace Azalea.Audio.OpenAL;
internal readonly struct ALC_Context
{
	public readonly IntPtr NativePointer;

	public ALC_Context(IntPtr pointer)
	{
		NativePointer = pointer;
	}

	public static implicit operator IntPtr(ALC_Context context) => context.NativePointer;
	public static implicit operator ALC_Context(IntPtr pointer) => new(pointer);
}
