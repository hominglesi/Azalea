using Azalea.Inputs;
using System;
using System.Collections.Generic;

namespace Azalea.Platform.Windows;
internal class WinGamepad : IGamepad
{
	public IntPtr PreparsedData;
	public HidPCaps Capabilities;
	public HidPButtonCaps[] ButtonCapabilities;
	public HidPValueCaps[] ValueCapabilities;

	public Dictionary<int, ButtonState> Buttons = new();
	public GamepadDPad DPad = new();

	public GamepadAnalogStick LeftAnalogStick = new();
	public GamepadAnalogStick RightAnalogStick = new();

	public WinGamepad(IntPtr preparsedData, HidPCaps capabilities,
		HidPButtonCaps[] buttonCapabilities, HidPValueCaps[] valueCapabilities)
	{
		PreparsedData = preparsedData;
		Capabilities = capabilities;
		ButtonCapabilities = buttonCapabilities;
		ValueCapabilities = valueCapabilities;
	}

	public void Update()
	{
		foreach (var button in Buttons)
			button.Value.Update();

		DPad.Update();
	}

	public ButtonState GetButton(int button)
	{
		if (Buttons.ContainsKey(button))
			return Buttons[button];

		return ButtonState.Default;
	}

	public GamepadDPad GetDPad() => DPad;
	public GamepadAnalogStick GetLeftStick() => LeftAnalogStick;
	public GamepadAnalogStick GetRightStick() => RightAnalogStick;
}
