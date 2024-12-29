namespace Azalea.Inputs;
public interface IGamepad
{
	internal void Update();

	public ButtonState GetButton(int button);
	public GamepadDPad GetDPad();

	public GamepadAnalogStick GetLeftStick();
	public GamepadAnalogStick GetRightStick();
}
