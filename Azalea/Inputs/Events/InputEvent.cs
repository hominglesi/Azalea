using Azalea.Graphics;
using System;

namespace Azalea.Inputs.Events;
public abstract class InputEvent
{
	private InputState? _state;
	public InputState State
	{
		get
		{
			if (_state is not null)
				return _state;

			throw new Exception("Input state is not avalible.");
		}
		internal set { _state = value; }
	}
}
