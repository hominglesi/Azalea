using Azalea.Inputs;
using Silk.NET.Input;
using System.Numerics;
using SilkMouseButton = Silk.NET.Input.MouseButton;

namespace Azalea.Platform.Silk;

internal class SilkInputManager
{
	private readonly IInputContext _input;

	public IMouse? PrimaryMouse;

	public IKeyboard? PrimaryKeyboard;

	public SilkInputManager(IInputContext input)
	{
		_input = input;

		if (_input.Mice.Count >= 1)
			PrimaryMouse = _input.Mice[0];

		if (_input.Keyboards.Count >= 1)
			PrimaryKeyboard = _input.Keyboards[0];

		foreach (var mouse in _input.Mice)
		{
			mouse.MouseMove += processMouseMove;
			mouse.MouseDown += processMouseDown;
			mouse.MouseUp += processMouseUp;
		}
		foreach (var keyboard in _input.Keyboards)
		{
			keyboard.KeyDown += processKeyDown;
			keyboard.KeyUp += processKeyUp;
			keyboard.KeyChar += processTextInput;
		}
	}

	private void processMouseMove(IMouse mouse, Vector2 position)
	{
		Input.HandleMousePositionChange(position);
	}

	private void processMouseDown(IMouse mouse, SilkMouseButton button)
	{
		var buttonIndex = (int)button;
		if (buttonIndex > 4) return;

		Input.HandleMouseButtonStateChange((Inputs.MouseButton)buttonIndex, true);
	}

	private void processMouseUp(IMouse mouse, SilkMouseButton button)
	{
		var buttonIndex = (int)button;
		if (buttonIndex > 4) return;

		Input.HandleMouseButtonStateChange((Inputs.MouseButton)buttonIndex, false);
	}

	private void processKeyDown(IKeyboard keyboard, Key key, int _)
	{
		var pressedKey = key.ToAzaleaKey();
		Input.HandleKeyboardKeyStateChange(pressedKey, true);
	}

	private void processKeyUp(IKeyboard keyboard, Key key, int _)
	{
		var pressedKey = key.ToAzaleaKey();
		Input.HandleKeyboardKeyStateChange(pressedKey, false);
	}

	private void processTextInput(IKeyboard keyboard, char chr)
	{
		Input.HandleTextInput(chr);
	}
}
