using Azalea.Design.UserInterface;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Azalea.Inputs.Events;

public class MouseMoveEvent(Vector2 position) : InputEvent
{
	public readonly Vector2 Position = position;

	public void Deconstruct(out Vector2 position)
	{
		position = Position;
	}
}
