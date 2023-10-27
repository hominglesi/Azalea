using Azalea.Inputs;
using System.Collections.Generic;

namespace Azalea.Graphics.GLFW;
public static class GLFWExtentions
{
	public static Keys KeycodeToKey(int keycode)
	{
		return _keyDictionary[keycode];
	}

	private static Dictionary<int, Keys> _keyDictionary;
	static GLFWExtentions()
	{
		//These are direct mappings from the GLFW values
		//https://www.glfw.org/docs/latest/group__keys.html

		//We can assume that the number of glfw keys will be about the same as Azalea keys
		//so we can preload the amount and later trim excess
		//(If we ever do a cleanup of Keys we could make this be the exact amount without trimming)
		//(There may be a faster data type like a HashMap or something but I don't have time to check right now)
		_keyDictionary = new((int)Keys.Amount)
		{
			{ -1, Keys.Unknown },
			{ 32, Keys.Space },
			{ 39, Keys.Apostrophe },
			{ 44, Keys.Comma },
			{ 45, Keys.Minus},
			{ 46, Keys.Period },
			{ 47, Keys.Slash},

			{ 48, Keys.Number0 },
			{ 49, Keys.Number1 },
			{ 50, Keys.Number2 },
			{ 51, Keys.Number3 },
			{ 52, Keys.Number4 },
			{ 53, Keys.Number5 },
			{ 54, Keys.Number6 },
			{ 55, Keys.Number7 },
			{ 56, Keys.Number8 },
			{ 57, Keys.Number9 },

			{ 59, Keys.Semicolon },
			{ 61, Keys.Equal },

			{ 65, Keys.A },
			{ 66, Keys.B },
			{ 67, Keys.C },
			{ 68, Keys.D },
			{ 69, Keys.E },
			{ 70, Keys.F },
			{ 71, Keys.G },
			{ 72, Keys.H },
			{ 73, Keys.I },
			{ 74, Keys.J },
			{ 75, Keys.K },
			{ 76, Keys.L },
			{ 77, Keys.M },
			{ 78, Keys.N },
			{ 79, Keys.O },
			{ 80, Keys.P },
			{ 81, Keys.Q },
			{ 82, Keys.R },
			{ 83, Keys.S },
			{ 84, Keys.T },
			{ 85, Keys.U },
			{ 86, Keys.V },
			{ 87, Keys.W },
			{ 88, Keys.X },
			{ 89, Keys.Y },
			{ 90, Keys.Z },

			{ 91, Keys.BracketLeft },
			{ 92, Keys.BackSlash },
			{ 93, Keys.BracketRight },
			{ 96, Keys.Grave },

			{ 161, Keys.World1 },
			{ 162, Keys.World2 },

			{ 256, Keys.Escape },
			{ 257, Keys.Enter },
			{ 258, Keys.Tab },
			{ 259, Keys.Backspace },
			{ 260, Keys.Insert },
			{ 261, Keys.Delete },
			{ 262, Keys.Right },
			{ 263, Keys.Left },
			{ 264, Keys.Down },
			{ 265, Keys.Up },
			{ 266, Keys.PageUp },
			{ 267, Keys.PageDown },
			{ 268, Keys.Home },
			{ 269, Keys.End },
			{ 280, Keys.CapsLock },
			{ 281, Keys.ScrollLock },
			{ 282, Keys.NumLock },
			{ 283, Keys.PrintScreen },
			{ 284, Keys.Pause },

			{ 290, Keys.F1 },
			{ 291, Keys.F2 },
			{ 292, Keys.F3 },
			{ 293, Keys.F4 },
			{ 294, Keys.F5 },
			{ 295, Keys.F6 },
			{ 296, Keys.F7 },
			{ 297, Keys.F8 },
			{ 298, Keys.F9 },
			{ 299, Keys.F10 },
			{ 300, Keys.F11 },
			{ 301, Keys.F12 },
			{ 302, Keys.F13 },
			{ 303, Keys.F14 },
			{ 304, Keys.F15 },
			{ 305, Keys.F16 },
			{ 306, Keys.F17 },
			//We currently support only up to F17
			//{ 307, Keys.F18 },
			//{ 308, Keys.F19 },
			//{ 309, Keys.F20 },
			//{ 310, Keys.F21 },
			//{ 311, Keys.F22 },
			//{ 312, Keys.F23 },
			//{ 313, Keys.F24 },
			//{ 314, Keys.F25 }

			{ 320, Keys.Keypad0 },
			{ 321, Keys.Keypad1 },
			{ 322, Keys.Keypad2 },
			{ 323, Keys.Keypad3 },
			{ 324, Keys.Keypad4 },
			{ 325, Keys.Keypad5 },
			{ 326, Keys.Keypad6 },
			{ 327, Keys.Keypad7 },
			{ 328, Keys.Keypad8 },
			{ 329, Keys.Keypad9 },

			{ 330, Keys.KeypadDecimal },
			{ 331, Keys.KeypadDivide },
			{ 332, Keys.KeypadMultiply },
			{ 333, Keys.KeypadSubtract },
			{ 334, Keys.KeypadAdd },
			{ 335, Keys.KeypadEnter },
			//Only one guy has reported that this key exist
			//if we need it I will add it
			//{ 336, Keys.KeypadEqual },

			{ 340, Keys.ShiftLeft },
			{ 341, Keys.ControlLeft },
			{ 342, Keys.AltLeft },
			{ 343, Keys.SuperLeft },
			{ 344, Keys.ShiftRight },
			{ 345, Keys.ControlRight },
			{ 346, Keys.AltRight },
			{ 347, Keys.SuperRight },
			{ 348, Keys.Menu },
		};

		_keyDictionary.TrimExcess();
	}
}
