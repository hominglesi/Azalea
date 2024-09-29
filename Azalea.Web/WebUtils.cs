using Azalea.Inputs;

namespace Azalea.Web;

public static class WebUtils
{
	public static MouseButton TranslateMouseButton(int button)
		=> button switch
		{
			1 => MouseButton.Middle,
			2 => MouseButton.Right,
			3 => MouseButton.Button1,
			4 => MouseButton.Button2,
			_ => MouseButton.Left
		};

	public static Keys TranslateKey(string key)
	{
		if (key.Length == 1)
		{
			var chr = key[0];
			if (char.IsLetter(chr))
				return (Keys)char.ToUpper(chr);

			if (char.IsDigit(chr))
				return (Keys)(chr + 43);
		}

		return key switch
		{
			"Shift" => Keys.ShiftLeft,
			"Control" => Keys.ControlLeft,
			"Alt" => Keys.AltLeft,
			"Meta" => Keys.WinLeft,
			"F1" => Keys.F1,
			"F2" => Keys.F2,
			"F3" => Keys.F3,
			"F4" => Keys.F4,
			"F5" => Keys.F5,
			"F6" => Keys.F6,
			"F7" => Keys.F7,
			"F8" => Keys.F8,
			"F9" => Keys.F9,
			"F10" => Keys.F10,
			"F11" => Keys.F11,
			"F12" => Keys.F12,
			"F13" => Keys.F13,
			"F14" => Keys.F14,
			"F15" => Keys.F15,
			"F16" => Keys.F16,
			"F17" => Keys.F16,
			"ArrowUp" => Keys.Up,
			"ArrowDown" => Keys.Down,
			"ArrowLeft" => Keys.Left,
			"ArrowRight" => Keys.Right,
			"Enter" => Keys.Enter,
			"Escape" => Keys.Escape,
			" " => Keys.Space,
			"Tab" => Keys.Tab,
			"Backspace" => Keys.Backspace,
			"Insert" => Keys.Insert,
			"Delete" => Keys.Delete,
			"PageUp" => Keys.PageUp,
			"PageDown" => Keys.PageDown,
			"Home" => Keys.Home,
			"End" => Keys.End,
			"CapsLock" => Keys.CapsLock,
			"ScrollLock" => Keys.ScrollLock,
			"PrintScreen" => Keys.PrintScreen,
			"Pause" => Keys.Pause,
			"NumLock" => Keys.NumLock,
			"Clear" => Keys.Clear,
			"!" => Keys.Number1,
			"@" => Keys.Number2,
			"#" => Keys.Number3,
			"$" => Keys.Number4,
			"%" => Keys.Number5,
			"^" => Keys.Number6,
			"&" => Keys.Number7,
			"*" => Keys.Number8,
			"(" => Keys.Number9,
			")" => Keys.Number0,
			"`" => Keys.Grave,
			"~" => Keys.Grave,
			"-" => Keys.Minus,
			"_" => Keys.Minus,
			"=" => Keys.Plus,
			"+" => Keys.Plus,
			"{" => Keys.BracketLeft,
			"[" => Keys.BracketLeft,
			"}" => Keys.BracketRight,
			"]" => Keys.BracketRight,
			";" => Keys.Semicolon,
			":" => Keys.Semicolon,
			"'" => Keys.Quote,
			"\"" => Keys.Quote,
			"," => Keys.Comma,
			"<" => Keys.Comma,
			"." => Keys.Period,
			">" => Keys.Period,
			"/" => Keys.Slash,
			"?" => Keys.Slash,
			"\\" => Keys.BackSlash,
			"|" => Keys.BackSlash,
			_ => Keys.Unknown
		};
	}
}
