using System;
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

	public Vector2 GetVectorCircular()
	{
		var vector = GetVector();

		return new Vector2(
			vector.X * MathF.Sqrt(1 - MathF.Pow(0.5f * vector.Y, 2)),
			vector.Y * MathF.Sqrt(1 - MathF.Pow(0.5f * vector.X, 2))
			);
	}
}
