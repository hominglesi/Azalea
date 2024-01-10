namespace Azalea.Inputs;

public class ButtonState
{
	private bool _pressed;
	private bool _down;
	private bool _up;

	private bool _repeat;
	public bool Pressed => _pressed;
	public bool Released => !_pressed;
	public bool Repeat => _repeat;
	public bool Down => _down;
	public bool Up => _up;
	public bool DownOrRepeat => _down || _repeat;

	internal void SetState(bool pressed)
	{
		_pressed = pressed;

		if (pressed)
		{
			_down = true;
		}
		else
		{
			_up = true;
		}
	}

	internal void SetRepeat() => _repeat = true;

	internal void Update()
	{
		_up = false;
		_down = false;
		_repeat = false;
	}
}
