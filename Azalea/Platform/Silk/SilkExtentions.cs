using SilkWindowState = Silk.NET.Windowing.WindowState;

namespace Azalea.Platform.Silk;

public static class SilkExtentions
{
    public static WindowState ToAzaleaWindowState(this SilkWindowState state)
    {
        return state switch
        {
            SilkWindowState.Fullscreen => WindowState.Fullscreen,
            _ => WindowState.Normal
        };
    }

    public static SilkWindowState ToSilkWindowState(this WindowState state)
    {
        return state switch
        {
            WindowState.Fullscreen => SilkWindowState.Fullscreen,
            WindowState.BorderlessFullscreen => SilkWindowState.Fullscreen,
            _ => SilkWindowState.Normal,
        };
    }
}
