using Veldrid;
using AzaleaKeys = Azalea.Inputs.Keys;
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

    public static AzaleaKeys ToAzaleaKey(this Key key)
    {
        if (key >= Key.A && key <= Key.Z) return (AzaleaKeys)(((int)key) - 18);
        if (key >= Key.F1 && key <= Key.F25) return (AzaleaKeys)(((int)key) + 280);
        if (key >= Key.Keypad0 && key <= Key.Keypad9) return (AzaleaKeys)(((int)key) + 253);
        if (key >= Key.Number0 && key <= Key.Number9) return (AzaleaKeys)(((int)key) - 61);

        return key switch
        {
            Key.ShiftLeft => AzaleaKeys.ShiftLeft,
            Key.ShiftRight => AzaleaKeys.ShiftRight,
            Key.ControlLeft => AzaleaKeys.ControlLeft,
            Key.ControlRight => AzaleaKeys.ControlRight,
            Key.AltLeft => AzaleaKeys.AltLeft,
            Key.AltRight => AzaleaKeys.AltRight,
            Key.WinLeft => AzaleaKeys.WinLeft,
            Key.WinRight => AzaleaKeys.WinRight,
            Key.Menu => AzaleaKeys.Menu,
            Key.Up => AzaleaKeys.Up,
            Key.Down => AzaleaKeys.Down,
            Key.Left => AzaleaKeys.Left,
            Key.Right => AzaleaKeys.Right,
            Key.Enter => AzaleaKeys.Enter,
            Key.Escape => AzaleaKeys.Escape,
            Key.Space => AzaleaKeys.Space,
            Key.Tab => AzaleaKeys.Tab,
            Key.BackSpace => AzaleaKeys.Backspace,
            Key.Insert => AzaleaKeys.Insert,
            Key.Delete => AzaleaKeys.Delete,
            Key.PageUp => AzaleaKeys.PageUp,
            Key.PageDown => AzaleaKeys.PageDown,
            Key.Home => AzaleaKeys.Home,
            Key.End => AzaleaKeys.End,
            Key.CapsLock => AzaleaKeys.CapsLock,
            Key.ScrollLock => AzaleaKeys.ScrollLock,
            Key.PrintScreen => AzaleaKeys.PrintScreen,
            Key.Pause => AzaleaKeys.Pause,
            Key.NumLock => AzaleaKeys.NumLock,
            Key.KeypadDivide => AzaleaKeys.KeypadDivide,
            Key.KeypadMultiply => AzaleaKeys.KeypadMultiply,
            Key.KeypadSubtract => AzaleaKeys.KeypadSubtract,
            Key.KeypadAdd => AzaleaKeys.KeypadAdd,
            Key.KeypadDecimal => AzaleaKeys.KeypadDecimal,
            Key.KeypadEnter => AzaleaKeys.KeypadEnter,
            Key.Grave => AzaleaKeys.GraveAccent,
            Key.Minus => AzaleaKeys.Minus,
            Key.BracketLeft => AzaleaKeys.LeftBracket,
            Key.BracketRight => AzaleaKeys.RightBracket,
            Key.Semicolon => AzaleaKeys.Semicolon,
            Key.Comma => AzaleaKeys.Comma,
            Key.Period => AzaleaKeys.Period,
            Key.Slash => AzaleaKeys.Slash,
            Key.BackSlash => AzaleaKeys.BackSlash,
            _ => AzaleaKeys.Unknown
        };
    }
}
