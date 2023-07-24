using Azalea.Graphics.Veldrid;
using Azalea.Inputs;
using System;
using Veldrid;
using Veldrid.Sdl2;

using AzaleaButtonState = Azalea.Inputs.ButtonState;

namespace Azalea.Platform.Veldrid;

internal class VeldridInputManager
{
    private Sdl2Window _window;

    public VeldridInputManager(Sdl2Window window)
    {
        _window = window;

        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < Input.MOUSE_BUTTONS.Length; i++)
        {
            Input.MOUSE_BUTTONS[i] = new AzaleaButtonState();
        }
        foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
        {
            if (Input.KEYBOARD_KEYS.ContainsKey((int)key)) continue;

            Input.KEYBOARD_KEYS.Add((int)key, new AzaleaButtonState());
        }

        _window.MouseMove += processMouseMove;
        _window.MouseDown += processMouseDown;
        _window.MouseUp += processMouseUp;

        _window.KeyDown += processKeyDown;
        _window.KeyUp += processKeyUp;
    }

    private void processKeyUp(KeyEvent obj)
    {
        var pressedKey = (int)obj.Key.ToAzaleaKey();
        if (Input.KEYBOARD_KEYS.ContainsKey(pressedKey) == false) return;
        Input.KEYBOARD_KEYS[pressedKey].SetUp();
    }

    private void processKeyDown(KeyEvent obj)
    {
        var pressedKey = (int)obj.Key.ToAzaleaKey();
        if (Input.KEYBOARD_KEYS.ContainsKey(pressedKey) == false) return;
        Input.KEYBOARD_KEYS[pressedKey].SetDown();
    }

    private void processMouseDown(MouseEvent obj)
    {
        var buttonIndex = (int)VeldridExtentions.ToAzaleaMouseInput(obj.MouseButton);
        if (buttonIndex > 4) return;

        Input.MOUSE_BUTTONS[buttonIndex].SetDown();
    }

    private void processMouseUp(MouseEvent obj)
    {
        var buttonIndex = (int)VeldridExtentions.ToAzaleaMouseInput(obj.MouseButton);
        if (buttonIndex > 4) return;

        Input.MOUSE_BUTTONS[buttonIndex].SetUp();
    }

    private void processMouseMove(MouseMoveEventArgs obj)
    {
        Input.MOUSE_POSITION = obj.MousePosition;
    }

    public void Update()
    {
        foreach (var key in Input.KEYBOARD_KEYS)
        {
            key.Value.Update();
        }

        foreach (var mouseButton in Input.MOUSE_BUTTONS)
        {
            mouseButton.Update();
        }
    }
}
