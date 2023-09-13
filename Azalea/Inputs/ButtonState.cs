using Azalea.Platform;

namespace Azalea.Inputs;

public class ButtonState
{
	private const float RepeatDelay = 500;
	private const float RepeatRate = 30;

	private bool _pressed;
	private bool _down;
	private bool _up;

	private float _heldTime;
	private bool _repeat;
	public bool Pressed => _pressed;
	public bool Released => !_pressed;
	public bool Repeat => _repeat;
	public bool Down => _down;
	public bool Up => _up;
	public bool DownOrRepeat => _down || _repeat;

	internal void SetDown()
	{
		_pressed = true;
		_down = true;

		_heldTime = 0;
	}

	internal void SetUp()
	{
		_pressed = false;
		_up = true;
	}

	internal void Update()
	{
		if (_pressed) _heldTime += Time.DeltaTimeMs;

		if (_heldTime > RepeatDelay + RepeatRate)
		{
			_repeat = true;
			_heldTime = RepeatDelay;
		}
		else _repeat = false;

		_up = false;
		_down = false;
	}
}
