using Azalea.Graphics;
using Azalea.Inputs.States;
using System.Numerics;

namespace Azalea.Inputs.Events;

public abstract class UIEvent
{
	public InputState CurrentState;

	public GameObject? Target;

	protected Vector2 ToLocalSpace(Vector2 screenSpacePosition) => Target?.Parent?.ToLocalSpace(screenSpacePosition) ?? screenSpacePosition;

	public Vector2 ScreenSpaceMousePosition => CurrentState.Mouse.Position;

	protected UIEvent(InputState state)
	{
		CurrentState = state;
	}
}
