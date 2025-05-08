namespace Azalea.Inputs;
public interface IGamepad
{
	public ButtonState GetButton(GamepadButton button);
	public GamepadDPad GetDPad();

	public GamepadAnalogStick GetLeftStick();
	public GamepadAnalogStick GetRightStick();
}
