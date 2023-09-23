using Azalea.Platform;
using System;

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

	internal event Action<int>? OnRepeat;

	private int _index;
	public ButtonState(int index)
	{
		_index = index;
	}

	internal void SetState(bool pressed)
	{
		_pressed = pressed;

		if (pressed)
		{
			_down = true;
			_heldTime = 0;
		}
		else
		{
			_up = true;
		}
	}

	internal void Update()
	{
		if (_pressed) _heldTime += Time.DeltaTimeMs;

		if (_heldTime > RepeatDelay + RepeatRate)
		{
			_repeat = true;
			OnRepeat?.Invoke(_index);
			_heldTime = RepeatDelay;
		}
		else _repeat = false;

		_up = false;
		_down = false;
	}
}
