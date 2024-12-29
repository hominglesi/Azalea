using System.Numerics;

namespace Azalea.Inputs;
public class GamepadAnalogStick
{
	public float Horizontal { get; internal set; }
	public float Vertical { get; internal set; }

	public Vector2 GetVector()
		=> new(Horizontal, Vertical);

	public Vector2 GetVectorNormalized()
	{
		var vector = GetVector();
		if (vector == Vector2.Zero)
			return vector;

		return Vector2.Normalize(vector);
	}
}
