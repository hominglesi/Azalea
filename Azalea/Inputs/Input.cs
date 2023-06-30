using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Inputs;

public static class Input
{
    internal static Vector2 MOUSE_POSITION;
    internal static ButtonState[] MOUSE_BUTTONS = new ButtonState[5];

    internal static Dictionary<int, ButtonState> KEYBOARD_KEYS = new();

    public static Vector2 MousePosition => MOUSE_POSITION;

    public static ButtonState GetKey(Keys key) => GetKey((int)key);
    public static ButtonState GetKey(int keycode) => KEYBOARD_KEYS[keycode];
    public static ButtonState GetMouseButton(int index) => MOUSE_BUTTONS[index];
}
