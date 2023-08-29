using Veldrid.Sdl2;

namespace Azalea.Platform.Veldrid;

public class VeldridClipboard : IClipboard
{
	public string? GetText() => Sdl2Native.SDL_GetClipboardText();

	public void SetText(string text) => Sdl2Native.SDL_SetClipboardText(text);
}
