using Silk.NET.Input;
using AzaleaKeys = Azalea.Inputs.Keys;
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

	public static AzaleaKeys ToAzaleaKey(this Key key)
	{
		if (key >= Key.A && key <= Key.Z) return (AzaleaKeys)key;
		if (key >= Key.Keypad0 && key <= Key.Keypad9) return (AzaleaKeys)(((int)key) - 271);
		if (key >= Key.Number0 && key <= Key.Number9) return (AzaleaKeys)(((int)key) + 43);
		if (key >= Key.F1 && key <= Key.F17) return (AzaleaKeys)(((int)key) - 280);

		return key switch
		{
			Key.ShiftLeft => AzaleaKeys.ShiftLeft,
			Key.ShiftRight => AzaleaKeys.ShiftRight,
			Key.ControlLeft => AzaleaKeys.ControlLeft,
			Key.ControlRight => AzaleaKeys.ControlRight,
			Key.AltLeft => AzaleaKeys.AltLeft,
			Key.AltRight => AzaleaKeys.AltRight,
			Key.SuperLeft => AzaleaKeys.WinLeft,
			Key.SuperRight => AzaleaKeys.WinRight,
			Key.Up => AzaleaKeys.Up,
			Key.Down => AzaleaKeys.Down,
			Key.Left => AzaleaKeys.Left,
			Key.Right => AzaleaKeys.Right,
			Key.Enter => AzaleaKeys.Enter,
			Key.Escape => AzaleaKeys.Escape,
			Key.Space => AzaleaKeys.Space,
			Key.Tab => AzaleaKeys.Tab,
			Key.Backspace => AzaleaKeys.Backspace,
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
			//Key.Clear => AzaleaKeys.Clear,
			//Key.Sleep => AzaleaKeys.Sleep,
			Key.KeypadDivide => AzaleaKeys.KeypadDivide,
			Key.KeypadMultiply => AzaleaKeys.KeypadMultiply,
			Key.KeypadSubtract => AzaleaKeys.KeypadMinus,
			Key.KeypadAdd => AzaleaKeys.KeypadPlus,
			Key.KeypadDecimal => AzaleaKeys.KeypadPeriod,
			Key.KeypadEnter => AzaleaKeys.KeypadEnter,
			Key.GraveAccent => AzaleaKeys.Tilde,
			Key.Minus => AzaleaKeys.Minus,
			//Key.Plus => AzaleaKeys.Plus,
			Key.LeftBracket => AzaleaKeys.BracketLeft,
			Key.RightBracket => AzaleaKeys.BracketRight,
			Key.Semicolon => AzaleaKeys.Semicolon,
			//Key.Quote => AzaleaKeys.Quote,
			Key.Comma => AzaleaKeys.Comma,
			Key.Period => AzaleaKeys.Period,
			Key.Slash => AzaleaKeys.Slash,
			Key.BackSlash => AzaleaKeys.BackSlash,
			_ => AzaleaKeys.Unknown
		};
	}
}
