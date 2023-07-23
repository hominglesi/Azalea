using Azalea.Inputs;
using System;
using Veldrid.Sdl2;
using MouseButtonVeldrid = Veldrid.MouseButton;

namespace Azalea.Platform.Veldrid;

public static class VeldridExtentions
{
    public static MouseButton ToAzaleaMouseInput(MouseButtonVeldrid input)
    {
        switch (input)
        {
            case MouseButtonVeldrid.Middle: return MouseButton.Middle;
            case MouseButtonVeldrid.Right: return MouseButton.Right;
            default: return (MouseButton)input;
        }
    }

    private unsafe delegate SDL_Surface SDL_CreateRGBSurfaceFrom_t(void* pixels, int width, int height, int depth, int pitch, uint Rmask, uint Gmask, uint Bmask, uint Amask);
    private static SDL_CreateRGBSurfaceFrom_t s_createRGBSurfaceFrom = Sdl2Native.LoadFunction<SDL_CreateRGBSurfaceFrom_t>("SDL_CreateRGBSurfaceFrom");
    public unsafe static SDL_Surface SDL_CreateRGBSurfaceFrom(void* pixels, int width, int height, int depth, int pitch, uint Rmask, uint Gmask, uint Bmask, uint Amask)
        => s_createRGBSurfaceFrom(pixels, width, height, depth, pitch, Rmask, Gmask, Bmask, Amask);

    private unsafe delegate void SDL_SetWindowIcon_t(SDL_Window window, SDL_Surface surface);
    private static SDL_SetWindowIcon_t s_setWindowIcon = Sdl2Native.LoadFunction<SDL_SetWindowIcon_t>("SDL_SetWindowIcon");
    public unsafe static void SDL_SetWindowIcon(SDL_Window window, SDL_Surface surface) => s_setWindowIcon(window, surface);

    private unsafe delegate void SDL_FreeSurface_t(IntPtr surface);
    private static SDL_FreeSurface_t s_freeSurface = Sdl2Native.LoadFunction<SDL_FreeSurface_t>("SDL_FreeSurface");
    public unsafe static void SDL_FreeSurface(IntPtr surface) => s_freeSurface(surface);
}
