using System;
using System.Numerics;

namespace Azalea.Inputs;
public class GLFWJoystick : IJoystick
{
	internal readonly int Handle;
	public string Name { get; init; }
	private Vector2[] _axies = Array.Empty<Vector2>();

	public GLFWJoystick(int handle, string name)
	{
		Handle = handle;
		Name = name;
	}

	internal void SetAxies(Vector2[] axies)
	{
		_axies = axies;
	}

	public JoystickAxis GetAxis(int axis)
	{
		if (axis < _axies.Length)
			return new JoystickAxis(_axies[axis]);
		else
			return new JoystickAxis(Vector2.Zero);
	}
}
