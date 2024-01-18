using Azalea.Inputs;
using System.Collections.Generic;

namespace Azalea.Platform.Windows;
internal static class WindowsExtentions
{
	public static Keys KeycodeToKey(int keycode)
	{
		if (_keyDictionary.ContainsKey(keycode) == false) return 0;

		return _keyDictionary[keycode];
	}

	private static Dictionary<int, Keys> _keyDictionary;
	static WindowsExtentions()
	{
		//These are direct mappings from the virtual-key values
		//https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

		//We can assume that the number of virtual keys will be about the same as Azalea keys
		//so we can preload the amount and later trim excess
		//(If we ever do a cleanup of Keys we could make this be the exact amount without trimming)
		//(There may be a faster data type like a HashMap or something but I don't have time to check right now)
		_keyDictionary = new((int)Keys.Amount)
		{
			{ 0x08, Keys.Backspace },
			{ 0x09, Keys.Tab },
			{ 0x0C, Keys.Clear },
			{ 0x0D, Keys.Enter },
			{ 0x13, Keys.Pause },
			{ 0x14, Keys.CapsLock },
			{ 0x1B, Keys.Escape },
			{ 0x20, Keys.Space },
			{ 0x21, Keys.PageUp },
			{ 0x22, Keys.PageDown },
			{ 0x23, Keys.End },
			{ 0x24, Keys.Home },
			{ 0x25, Keys.Left },
			{ 0x26, Keys.Up },
			{ 0x27, Keys.Right },
			{ 0x28, Keys.Down },
			{ 0x2C, Keys.PrintScreen },
			{ 0x2D, Keys.Insert },
			{ 0x2E, Keys.Delete },

			{ 0x30, Keys.Number0 },
			{ 0x31, Keys.Number1 },
			{ 0x32, Keys.Number2 },
			{ 0x33, Keys.Number3 },
			{ 0x34, Keys.Number4 },
			{ 0x35, Keys.Number5 },
			{ 0x36, Keys.Number6 },
			{ 0x37, Keys.Number7 },
			{ 0x38, Keys.Number8 },
			{ 0x39, Keys.Number9 },

			{ 0x41, Keys.A },
			{ 0x42, Keys.B },
			{ 0x43, Keys.C },
			{ 0x44, Keys.D },
			{ 0x45, Keys.E },
			{ 0x46, Keys.F },
			{ 0x47, Keys.G },
			{ 0x48, Keys.H },
			{ 0x49, Keys.I },
			{ 0x4A, Keys.J },
			{ 0x4B, Keys.K },
			{ 0x4C, Keys.L },
			{ 0x4D, Keys.M },
			{ 0x4E, Keys.N },
			{ 0x4F, Keys.O },
			{ 0x50, Keys.P },
			{ 0x51, Keys.Q },
			{ 0x52, Keys.R },
			{ 0x53, Keys.S },
			{ 0x54, Keys.T },
			{ 0x55, Keys.U },
			{ 0x56, Keys.V },
			{ 0x57, Keys.W },
			{ 0x58, Keys.X },
			{ 0x59, Keys.Y },
			{ 0x5A, Keys.Z },

			{ 0x5B, Keys.WinLeft },
			{ 0x5C, Keys.WinRight },

			{ 0x60, Keys.Keypad0 },
			{ 0x61, Keys.Keypad1 },
			{ 0x62, Keys.Keypad2 },
			{ 0x63, Keys.Keypad3 },
			{ 0x64, Keys.Keypad4 },
			{ 0x65, Keys.Keypad5 },
			{ 0x66, Keys.Keypad6 },
			{ 0x67, Keys.Keypad7 },
			{ 0x68, Keys.Keypad8 },
			{ 0x69, Keys.Keypad9 },

			{ 0x6A, Keys.KeypadMultiply },
			{ 0x6B, Keys.KeypadAdd },
			{ 0x6D, Keys.KeypadSubtract },
			{ 0x6E, Keys.KeypadDecimal },
			{ 0x6F, Keys.KeypadDivide },

			{ 0x70, Keys.F1 },
			{ 0x71, Keys.F2 },
			{ 0x72, Keys.F3 },
			{ 0x73, Keys.F4 },
			{ 0x74, Keys.F5 },
			{ 0x75, Keys.F6 },
			{ 0x76, Keys.F7 },
			{ 0x77, Keys.F8 },
			{ 0x78, Keys.F9 },
			{ 0x79, Keys.F10 },
			{ 0x7A, Keys.F11 },
			{ 0x7B, Keys.F12 },
			{ 0x7C, Keys.F13 },
			{ 0x7D, Keys.F14 },
			{ 0x7E, Keys.F15 },
			{ 0x7F, Keys.F16 },
			{ 0x80, Keys.F17 },

			{ 0x90, Keys.NumLock },
			{ 0x91, Keys.ScrollLock },
			{ 0xA0, Keys.ShiftLeft },
			{ 0xA1, Keys.ShiftRight },
			{ 0xA2, Keys.ControlLeft },
			{ 0xA3, Keys.ControlRight },
			{ 0xA4, Keys.AltLeft },
			{ 0xA5, Keys.AltRight },

			{ 0xC0, Keys.Grave },
			{ 0xDC, Keys.BackSlash },
			{ 0xBB, Keys.Plus },
			{ 0xBD, Keys.Minus },
			{ 0xDB, Keys.BracketLeft },
			{ 0xDD, Keys.BracketRight },
			{ 0xBA, Keys.Semicolon },
			{ 0xDE, Keys.Apostrophe },
			{ 0xBC, Keys.Comma },
			{ 0xBE, Keys.Period },
			{ 0xBF, Keys.Slash },

			//Temporary
			{ 0x10, Keys.ShiftLeft },
			{ 0x11, Keys.ControlLeft },
			{ 0x12, Keys.AltLeft }
		};

		_keyDictionary.TrimExcess();
	}
}
