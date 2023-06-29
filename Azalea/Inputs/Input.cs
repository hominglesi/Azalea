using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Inputs;

public static class Input
{
    internal static Vector2 MOUSE_POSITION;
    internal static List<Keys> PRESSED_KEYS = new();

    public static Vector2 MousePosition => MOUSE_POSITION;
    public static List<Keys> PressedKeys => PRESSED_KEYS;

    public static bool IsKeyPressed(Keys key)
    {
        return PRESSED_KEYS.Contains(key);
    }
}
