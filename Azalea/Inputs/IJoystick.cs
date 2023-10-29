using System.Numerics;

namespace Azalea.Inputs;
public interface IJoystick
{
	public string Name { get; }

	public JoystickAxis GetAxis(int axis);
}

public readonly struct JoystickAxis
{
	public Vector2 Direction { get; }
	public Vector2 NormalizedDirection { get; }

	public JoystickAxis(Vector2 direction)
	{
		Direction = direction;

		var normalizedX = direction.X == 0 ? 0 : direction.X > 0 ? 1 : -1;
		var normalizedY = direction.Y == 0 ? 0 : direction.Y > 0 ? 1 : -1;
		NormalizedDirection = new Vector2(normalizedX, normalizedY);
	}
}
