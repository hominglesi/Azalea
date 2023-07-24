using Veldrid;

using AzaleaWindowState = Azalea.Platform.WindowState;

namespace Azalea.Graphics.Veldrid;

internal static class VeldridExtentions
{
    public static RgbaFloat ToRgbaFloat(this Color color) => new(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);

    public static WindowState ToVeldridWindowState(this AzaleaWindowState state)
    {
        return state switch
        {
            AzaleaWindowState.Normal => WindowState.Normal,
            AzaleaWindowState.Fullscreen => WindowState.FullScreen,
            AzaleaWindowState.BorderlessFullscreen => WindowState.BorderlessFullScreen,
            _ => WindowState.Normal,
        };
    }

    public static AzaleaWindowState ToAzaleaWindowState(this WindowState state)
    {
        return state switch
        {
            WindowState.Normal => AzaleaWindowState.Normal,
            WindowState.FullScreen => AzaleaWindowState.Fullscreen,
            WindowState.BorderlessFullScreen => AzaleaWindowState.BorderlessFullscreen,
            _ => AzaleaWindowState.Normal
        };
    }
}
