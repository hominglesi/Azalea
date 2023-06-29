using Azalea.Inputs;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Platform.Silk;

internal class SilkInputManager
{
    private readonly IInputContext _input;

    public SilkInputManager(IInputContext input)
    {
        _input = input;

        Initialize();
    }

    private void Initialize()
    {
        foreach (var mouse in _input.Mice)
        {
            mouse.MouseMove += ProcessMouseMove;
        }
        foreach (var keyboard in _input.Keyboards)
        {
            keyboard.KeyDown += ProcessKeyDown;
            keyboard.KeyUp += ProcessKeyUp;
        }
    }

    private void ProcessMouseMove(IMouse mouse, Vector2 position)
    {
        Input.MOUSE_POSITION = position;
    }

    private void ProcessKeyDown(IKeyboard keyboard, Key key, int _)
    {
        var pressedKey = (Keys)key;
        if (Input.PRESSED_KEYS.Contains(pressedKey) == false)
            Input.PRESSED_KEYS.Add(pressedKey);
    }

    private void ProcessKeyUp(IKeyboard keyboard, Key key, int _)
    {
        var pressedKey = (Keys)key;
        if (Input.PRESSED_KEYS.Contains(pressedKey))
            Input.PRESSED_KEYS.Remove(pressedKey);
    }
}
