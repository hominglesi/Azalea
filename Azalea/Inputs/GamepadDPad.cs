using System.Numerics;

namespace Azalea.Inputs;
public class GamepadDPad
{
	private float _horizontal;
	private float _vertical;

	public readonly ButtonState Up = new();
	public readonly ButtonState Down = new();
	public readonly ButtonState Left = new();
	public readonly ButtonState Right = new();

	internal void SetDirection(float horizontal, float vertical)
	{
		if (vertical == -1 && Up.Released) Up.SetState(true);
		else if (vertical != -1 && Up.Pressed) Up.SetState(false);

		if (vertical == 1 && Down.Released) Down.SetState(true);
		else if (vertical != 1 && Down.Pressed) Down.SetState(false);

		if (horizontal == 1 && Right.Released) Right.SetState(true);
		else if (horizontal != 1 && Right.Pressed) Right.SetState(false);

		if (horizontal == -1 && Left.Released) Left.SetState(true);
		else if (horizontal != -1 && Left.Pressed) Left.SetState(false);

		_horizontal = horizontal;
		_vertical = vertical;
	}

	internal void SetIndividual(bool up, bool down, bool left, bool right)
	{
		Up.SetState(up);
		Down.SetState(down);
		Left.SetState(left);
		Right.SetState(right);

		if (up)
			_vertical = -1;
		else if (down)
			_vertical = 1;
		else
			_vertical = 0;

		if (left)
			_horizontal = -1;
		else if (right)
			_horizontal = 1;
		else
			_horizontal = 0;
	}

	internal void Update()
	{
		Up.Update();
		Down.Update();
		Left.Update();
		Right.Update();
	}

	public Vector2 GetVector()
		=> new(_horizontal, _vertical);

	public Vector2 GetVectorNormalized()
	{
		var vector = GetVector();
		if (vector == Vector2.Zero)
			return vector;

		return Vector2.Normalize(vector);
	}
}
