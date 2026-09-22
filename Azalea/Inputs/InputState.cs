using Azalea.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Azalea.Inputs;

public class InputState
{
	public Vector2 MousePosition { get; internal set; }
	public GameObject? FocusedObject { get; internal set; }
	internal List<GameObject> HoveredObjectsInternal { get; } = [];
	public IReadOnlyList<GameObject> HoveredObjects => HoveredObjectsInternal;

	#region MouseButtons

	private readonly HashSet<MouseButton> _pressedMouseButton = [];

	public bool MouseButtonPressed(MouseButton button) => _pressedMouseButton.Contains(button);

	internal void SetMouseButtonPressed(MouseButton button, bool pressed)
	{
		if (pressed)
			_pressedMouseButton.Add(button);
		else
			_pressedMouseButton.Remove(button);
	}

	#endregion

	#region Keys

	private readonly HashSet<Keys> _pressedKeys = [];

	public bool KeyPressed(Keys key) => _pressedKeys.Contains(key);

	public bool ControlPressed => KeyPressed(Keys.ControlLeft) || KeyPressed(Keys.ControlRight);
	public bool ShiftPressed => KeyPressed(Keys.ShiftLeft) || KeyPressed(Keys.ShiftRight);

	public Vector2 GetDirectionalMovement()
	{
		var horizontal = 0;
		var vertical = 0;

		if (KeyPressed(Keys.W) || KeyPressed(Keys.Up))
			vertical -= 1;

		if (KeyPressed(Keys.S) || KeyPressed(Keys.Down))
			vertical += 1;

		if (KeyPressed(Keys.D) || KeyPressed(Keys.Right))
			horizontal += 1;

		if (KeyPressed(Keys.A) || KeyPressed(Keys.Left))
			horizontal -= 1;

		if (horizontal == 0 && vertical == 0)
			return Vector2.Zero;

		var direction = new Vector2(horizontal, vertical);
		return Vector2.Normalize(direction);
	}

	internal void SetKeyPressed(Keys key, bool pressed)
	{
		if (pressed)
			_pressedKeys.Add(key);
		else
			_pressedKeys.Remove(key);
	}

	public event Action<char>? OnCharInput;

	internal void TriggerCharInput(char c) => OnCharInput?.Invoke(c);

	#endregion
}
