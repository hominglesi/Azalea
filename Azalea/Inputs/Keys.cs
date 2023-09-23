namespace Azalea.Inputs;

//Loosely based on the Veldrid key layout

//
// Summary:
//     Represents the keys on a keyboard.
public enum Keys
{
	//
	// Summary:
	//     A key outside the known keys.
	Unknown = 0,
	//
	// Summary:
	//     The left shift key.
	ShiftLeft = 1,
	//
	// Summary:
	//     The right shift key.
	ShiftRight = 2,
	//
	// Summary:
	//     The left control key.
	ControlLeft = 3,
	//
	// Summary:
	//     The right control key.
	ControlRight = 4,
	//
	// Summary:
	//     The left alt key.
	AltLeft = 5,
	//
	// Summary:
	//     The right alt key.
	AltRight = 6,
	//
	// Summary:
	//     The left win key.
	WinLeft = 7,
	//
	// Summary:
	//     The right win key.
	WinRight = 8,
	//
	// Summary:
	//     The menu key.
	Menu = 9,
	//
	// Summary:
	//     The F1 key.
	F1 = 10,
	//
	// Summary:
	//     The F2 key.
	F2 = 11,
	//
	// Summary:
	//     The F3 key.
	F3 = 12,
	//
	// Summary:
	//     The F4 key.
	F4 = 13,
	//
	// Summary:
	//     The F5 key.
	F5 = 14,
	//
	// Summary:
	//     The F6 key.
	F6 = 15,
	//
	// Summary:
	//     The F7 key.
	F7 = 16,
	//
	// Summary:
	//     The F8 key.
	F8 = 17,
	//
	// Summary:
	//     The F9 key.
	F9 = 18,
	//
	// Summary:
	//     The F10 key.
	F10 = 19,
	//
	// Summary:
	//     The F11 key.
	F11 = 20,
	//
	// Summary:
	//     The F12 key.
	F12 = 21,
	//
	// Summary:
	//		The F13 key
	F13 = 22,
	//
	// Summary:
	//		The F14 key
	F14 = 23,
	//
	// Summary:
	//		The F15 key
	F15 = 24,
	//
	// Summary:
	//		The F16 key
	F16 = 25,
	//
	// Summary:
	//		The F17 key
	F17 = 26,
	//
	// Summary:
	//     The up arrow key.
	Up = 27,
	//
	// Summary:
	//     The down arrow key.
	Down = 28,
	//
	// Summary:
	//     The left arrow key.
	Left = 29,
	//
	// Summary:
	//     The right arrow key.
	Right = 30,
	//
	// Summary:
	//     The enter key.
	Enter = 31,
	//
	// Summary:
	//     The escape key.
	Escape = 32,
	//
	// Summary:
	//     The space key.
	Space = 33,
	//
	// Summary:
	//     The tab key.
	Tab = 34,
	//
	// Summary:
	//     The backspace key.
	Backspace = 35,
	//
	// Summary:
	//     The insert key.
	Insert = 36,
	//
	// Summary:
	//     The delete key.
	Delete = 37,
	//
	// Summary:
	//     The page up key.
	PageUp = 38,
	//
	// Summary:
	//     The page down key.
	PageDown = 39,
	//
	// Summary:
	//     The home key.
	Home = 40,
	//
	// Summary:
	//     The end key.
	End = 41,
	//
	// Summary:
	//     The caps lock key.
	CapsLock = 42,
	//
	// Summary:
	//     The scroll lock key.
	ScrollLock = 43,
	//
	// Summary:
	//     The print screen key.
	PrintScreen = 44,
	//
	// Summary:
	//     The pause key.
	Pause = 45,
	//
	// Summary:
	//     The num lock key.
	NumLock = 46,
	//
	// Summary:
	//     The clear key
	Clear = 47,
	//
	// Summary:
	//     The sleep key.
	Sleep = 48,
	//
	// Summary:
	//     The keypad 0 key.
	Keypad0 = 49,
	//
	// Summary:
	//     The keypad 1 key.
	Keypad1 = 50,
	//
	// Summary:
	//     The keypad 2 key.
	Keypad2 = 51,
	//
	// Summary:
	//     The keypad 3 key.
	Keypad3 = 52,
	//
	// Summary:
	//     The keypad 4 key.
	Keypad4 = 53,
	//
	// Summary:
	//     The keypad 5 key.
	Keypad5 = 54,
	//
	// Summary:
	//     The keypad 6 key.
	Keypad6 = 55,
	//
	// Summary:
	//     The keypad 7 key.
	Keypad7 = 56,
	//
	// Summary:
	//     The keypad 8 key.
	Keypad8 = 57,
	//
	// Summary:
	//     The keypad 9 key.
	Keypad9 = 58,
	//
	// Summary:
	//     The keypad divide key.
	KeypadDivide = 59,
	//
	// Summary:
	//     The keypad multiply key.
	KeypadMultiply = 60,
	//
	// Summary:
	//     The keypad minus key.
	KeypadMinus = 61,
	//
	// Summary:
	//     The keypad plus key.
	KeypadPlus = 62,
	//
	// Summary:
	//     The keypad period key.
	KeypadPeriod = 63,
	//
	// Summary:
	//     The keypad enter key.
	KeypadEnter = 64,
	//
	// Summary:
	//     The A key.
	A = 65,
	//
	// Summary:
	//     The B key.
	B = 66,
	//
	// Summary:
	//     The C key.
	C = 67,
	//
	// Summary:
	//     The D key.
	D = 68,
	//
	// Summary:
	//     The E key.
	E = 69,
	//
	// Summary:
	//     The F key.
	F = 70,
	//
	// Summary:
	//     The G key.
	G = 71,
	//
	// Summary:
	//     The H key.
	H = 72,
	//
	// Summary:
	//     The I key.
	I = 73,
	//
	// Summary:
	//     The J key.
	J = 74,
	//
	// Summary:
	//     The K key.
	K = 75,
	//
	// Summary:
	//     The L key.
	L = 76,
	//
	// Summary:
	//     The M key.
	M = 77,
	//
	// Summary:
	//     The N key.
	N = 78,
	//
	// Summary:
	//     The O key.
	O = 79,
	//
	// Summary:
	//     The P key.
	P = 80,
	//
	// Summary:
	//     The Q key.
	Q = 81,
	//
	// Summary:
	//     The R key.
	R = 82,
	//
	// Summary:
	//     The S key.
	S = 83,
	//
	// Summary:
	//     The T key.
	T = 84,
	//
	// Summary:
	//     The U key.
	U = 85,
	//
	// Summary:
	//     The V key.
	V = 86,
	//
	// Summary:
	//     The W key.
	W = 87,
	//
	// Summary:
	//     The X key.
	X = 88,
	//
	// Summary:
	//     The Y key.
	Y = 89,
	//
	// Summary:
	//     The Z key.
	Z = 90,
	//
	// Summary:
	//     The number 0 key.
	Number0 = 91,
	//
	// Summary:
	//     The number 1 key.
	Number1 = 92,
	//
	// Summary:
	//     The number 2 key.
	Number2 = 93,
	//
	// Summary:
	//     The number 3 key.
	Number3 = 94,
	//
	// Summary:
	//     The number 4 key.
	Number4 = 95,
	//
	// Summary:
	//     The number 5 key.
	Number5 = 96,
	//
	// Summary:
	//     The number 6 key.
	Number6 = 97,
	//
	// Summary:
	//     The number 7 key.
	Number7 = 98,
	//
	// Summary:
	//     The number 8 key.
	Number8 = 99,
	//
	// Summary:
	//     The number 9 key.
	Number9 = 100,
	//
	// Summary:
	//     The tilde key.
	Tilde = 101,
	//
	// Summary:
	//     The minus key.
	Minus = 102,
	//
	// Summary:
	//     The plus key.
	Plus = 103,
	//
	// Summary:
	//     The left bracket key.
	BracketLeft = 104,
	//
	// Summary:
	//     The right bracket key.
	BracketRight = 105,
	//
	// Summary:
	//     The semicolon key.
	Semicolon = 106,
	//
	// Summary:
	//     The quote key.
	Quote = 107,
	//
	// Summary:
	//     The comma key.
	Comma = 108,
	//
	// Summary:
	//     The period key.
	Period = 109,
	//
	// Summary:
	//     The slash key.
	Slash = 110,
	//
	// Summary:
	//     The backslash key.
	BackSlash = 111,
	//
	// Summary:
	//     Indicates the last available keyboard key.
	LastKey = 112
}
