using Azalea.Inputs;
using Silk.NET.Input;
using System;
using System.Numerics;

using SilkMouseButton = Silk.NET.Input.MouseButton;

namespace Azalea.Platform.Silk;

internal class SilkInputManager
{
	private readonly IInputContext _input;

	public IMouse? PrimaryMouse;

	public SilkInputManager(IInputContext input)
	{
		_input = input;

		Initialize();
	}

	private void Initialize()
	{
		for (int i = 0; i < Input.MOUSE_BUTTONS.Length; i++)
		{
			Input.MOUSE_BUTTONS[i] = new ButtonState();
		}
		foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
		{
			if (Input.KEYBOARD_KEYS.ContainsKey((int)key)) continue;

			Input.KEYBOARD_KEYS.Add((int)key, new ButtonState());
		}

		Input.TEXT_INPUT_SOURCE = new TextInputSource();

		if (_input.Mice.Count >= 1)
			PrimaryMouse = _input.Mice[0];

		foreach (var mouse in _input.Mice)
		{
			mouse.MouseMove += ProcessMouseMove;
			mouse.MouseDown += ProcessMouseDown;
			mouse.MouseUp += ProcessMouseUp;
		}
		foreach (var keyboard in _input.Keyboards)
		{
			keyboard.KeyDown += ProcessKeyDown;
			keyboard.KeyUp += ProcessKeyUp;
			keyboard.KeyChar += ProcessTextInput;
		}
	}

	private void ProcessMouseMove(IMouse mouse, Vector2 position)
	{
		Input.MOUSE_POSITION = position;
	}

	private void ProcessMouseDown(IMouse mouse, SilkMouseButton button)
	{
		var buttonIndex = (int)button;
		if (buttonIndex > 4) return;

		Input.MOUSE_BUTTONS[buttonIndex].SetDown();
	}

	private void ProcessMouseUp(IMouse mouse, SilkMouseButton button)
	{
		var buttonIndex = (int)button;
		if (buttonIndex > 4) return;

		Input.MOUSE_BUTTONS[buttonIndex].SetUp();
	}

	private void ProcessKeyDown(IKeyboard keyboard, Key key, int _)
	{
		var pressedKey = (int)key;
		if (Input.KEYBOARD_KEYS.ContainsKey(pressedKey) == false) return;
		Input.KEYBOARD_KEYS[pressedKey].SetDown();
	}

	private void ProcessKeyUp(IKeyboard keyboard, Key key, int _)
	{
		var pressedKey = (int)key;
		if (Input.KEYBOARD_KEYS.ContainsKey(pressedKey) == false) return;
		Input.KEYBOARD_KEYS[pressedKey].SetUp();
	}

	private void ProcessTextInput(IKeyboard keyboard, char chr)
	{
		Input.TEXT_INPUT_SOURCE.TriggerTextInput(chr.ToString());
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
