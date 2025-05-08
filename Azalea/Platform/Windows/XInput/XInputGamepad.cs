using Azalea.Inputs;
using Azalea.Utils;

namespace Azalea.Platform.Windows.XInput;
internal class XInputGamepad : IGamepad
{
	private readonly ButtonState[] _buttons = new ButtonState[12];
	private readonly GamepadDPad _dPad = new();
	private readonly GamepadAnalogStick _leftStick = new();
	private readonly GamepadAnalogStick _rightStick = new();

	public bool IsConnected { get; set; } = false;

	public XInputGamepad()
	{
		for (int i = 0; i < _buttons.Length; i++)
			_buttons[i] = new ButtonState();
	}

	public void Update(XInputGamepadData data)
	{
		IsConnected = true;

		foreach (var button in _buttons)
			button.Update();

		_dPad.Update();

		_buttons[0].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 13));
		_buttons[1].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 14));
		_buttons[2].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 15));
		_buttons[3].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 16));
		_buttons[4].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 7));
		_buttons[5].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 8));
		_buttons[6].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 6));
		_buttons[7].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 5));
		_buttons[8].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 9));
		_buttons[9].SetState(BitwiseUtils.GetSpecificBit(data.Buttons, 10));
		_buttons[10].SetState(data.LeftTrigger > 0);
		_buttons[11].SetState(data.RightTrigger > 0);

		var dPadUp = BitwiseUtils.GetSpecificBit(data.Buttons, 1);
		var dPadDown = BitwiseUtils.GetSpecificBit(data.Buttons, 2);
		var dPadLeft = BitwiseUtils.GetSpecificBit(data.Buttons, 3);
		var dPadRight = BitwiseUtils.GetSpecificBit(data.Buttons, 4);

		_dPad.SetIndividual(dPadUp, dPadDown, dPadLeft, dPadRight);

		_leftStick.Horizontal = data.ThumbLX / (float)short.MaxValue;
		_leftStick.Vertical = data.ThumbLY / (float)short.MaxValue * -1;
		_rightStick.Horizontal = data.ThumbRX / (float)short.MaxValue;
		_rightStick.Vertical = data.ThumbRY / (float)short.MaxValue * -1;
	}

	public ButtonState GetButton(GamepadButton button) => _buttons[(int)button];
	public GamepadDPad GetDPad() => _dPad;
	public GamepadAnalogStick GetLeftStick() => _leftStick;
	public GamepadAnalogStick GetRightStick() => _rightStick;
}
