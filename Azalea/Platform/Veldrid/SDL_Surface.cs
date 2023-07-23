using System;

namespace Azalea.Platform.Veldrid;

public struct SDL_Surface
{
    public readonly IntPtr NativePointer;

    public SDL_Surface(IntPtr pointer)
    {
        NativePointer = pointer;
    }

    public static implicit operator IntPtr(SDL_Surface surface) => surface.NativePointer;
    public static implicit operator SDL_Surface(IntPtr pointer) => new SDL_Surface(pointer);
}
