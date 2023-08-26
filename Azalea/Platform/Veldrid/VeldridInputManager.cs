using Azalea.Graphics.Veldrid;
using Azalea.Inputs;
using System;
using Veldrid;
using Veldrid.Sdl2;

using AzaleaButtonState = Azalea.Inputs.ButtonState;

namespace Azalea.Platform.Veldrid;

internal class VeldridInputManager
{
	private Sdl2Window _sdl;

	public VeldridInputManager(VeldridWindow window)
	{
		_sdl = window.Window;
		window.OnInput += handleInput;

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

		Input.TEXT_INPUT_SOURCE = new TextInputSource();

		_sdl.MouseMove += processMouseMove;
		_sdl.MouseDown += processMouseDown;
		_sdl.MouseUp += processMouseUp;

		_sdl.KeyDown += processKeyDown;
		_sdl.KeyUp += processKeyUp;
	}

	private void handleInput()
	{
		var events = _sdl.PumpEvents();
		foreach (var charPress in events.KeyCharPresses)
		{
			Input.TEXT_INPUT_SOURCE.TriggerTextInput(charPress.ToString());
		}
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
