using Azalea.Design.Containers;
using Azalea.Editing;
using Azalea.Graphics;
using Azalea.Inputs.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Inputs;

public static class Input
{
	private static Composition? _rootObject;

	internal static void Initialize(Composition rootObject)
	{
		_rootObject = rootObject;

		var mouseButtonCount = (int)MouseButton.LastButton;
		_mouseButtons = new ButtonState[mouseButtonCount];

		for (int i = 0; i < mouseButtonCount; i++)
			_mouseButtons[i] = new ButtonState();

		var keyButtonCount = (int)Keys.Amount;
		_keyboardKeys = new ButtonState[keyButtonCount];

		for (int i = 0; i < keyButtonCount; i++)
			_keyboardKeys[i] = new ButtonState();
	}

	internal static void LateUpdate()
	{
		MouseWheelDelta = 0;

		foreach (var key in _keyboardKeys)
			key.Update();

		foreach (var mouseButton in _mouseButtons)
			mouseButton.Update();

		foreach (var gamepad in _gamepads)
			gamepad.Update();
	}

	/// <summary>
	/// Gets a list of all the objects in the scene graph that contain the specified position
	/// starting with the deepest ones and going out.
	/// </summary>
	public static IReadOnlyList<GameObject> GetPositionalInputQueue(Vector2 position)
	{
		Debug.Assert(_rootObject is not null);

		var inputQueue = new List<GameObject>();
		_rootObject.BuildPositionalInputQueue(position, inputQueue);
		inputQueue.Reverse();

		return inputQueue;
	}

	/// <summary>
	/// Gets a list of all the objects in the scene graph
	/// starting with the deepest ones and going out.
	/// </summary>
	public static IReadOnlyList<GameObject> GetNonPositionalInputQueue()
	{
		Debug.Assert(_rootObject is not null);

		var inputQueue = new List<GameObject>();
		_rootObject.BuildNonPositionalInputQueue(inputQueue);
		inputQueue.Reverse();

		return inputQueue;
	}

	private static void propagateNonPositionalInputEvent(InputEvent e)
	{
		foreach (var obj in GetNonPositionalInputQueue())
			if (obj.TriggerEvent(e) == true) return;
	}

	private static void propagatePositionalInputEvent(InputEvent e)
	{
		foreach (var obj in GetPositionalInputQueue(MousePosition))
			if (obj.TriggerEvent(e) == true) return;
	}

	#region Mouse

	private static Vector2 _lastMousePosition = Vector2.Zero;
	private static ButtonState[] _mouseButtons = Array.Empty<ButtonState>();
	private static readonly List<GameObject> _hoveredObjects = new();
	private static readonly List<GameObject> _lastHoveredObjects = new();
	private static GameObject? _hoverHandledObject;
	private static readonly List<GameObject> _clickDownGameObjects = new();

	/// <summary>
	/// Current mouse coordinates in screen space, meaning that (0, 0) is the top-left of the window client area.
	/// </summary>
	public static Vector2 MousePosition { get; private set; } = Vector2.Zero;

	/// <summary>
	/// Scroll value change since last frame.
	/// </summary>
	public static float MouseWheelDelta { get; private set; } = 0;

	/// <summary>
	/// Returns the state of the specified mouse button.
	/// </summary>
	public static ButtonState GetMouseButton(MouseButton button) => _mouseButtons[(int)button];

	/// <summary>
	/// Returns a read-only list of all the currently hovered objects.
	/// <paramref name="recalculate"/> specifies if the scene graph needs to be reevaluated for moved objects.
	/// </summary>
	public static IReadOnlyList<GameObject> GetHoveredObjects(bool recalculate = false)
	{
		if (recalculate) updateHoveredObjects();
		return _hoveredObjects;
	}

	private static void updateHoveredObjects()
	{
		GameObject? lastHoverHandledObject = _hoverHandledObject;
		_hoverHandledObject = null;

		_lastHoveredObjects.Clear();
		_lastHoveredObjects.AddRange(_hoveredObjects);

		_hoveredObjects.Clear();

		var positionalQueue = GetPositionalInputQueue(MousePosition);

		foreach (var obj in positionalQueue)
		{
			_hoveredObjects.Add(obj);
			_lastHoveredObjects.Remove(obj);

			if (obj.Hovered)
			{
				if (obj == lastHoverHandledObject)
				{
					_hoverHandledObject = lastHoverHandledObject;
					break;
				}

				continue;
			}

			obj.Hovered = true;

			if (obj.TriggerEvent(new HoverEvent()))
			{
				_hoverHandledObject = obj;
				break;
			}
		}

		foreach (var obj in _lastHoveredObjects)
		{
			obj.Hovered = false;
			obj.TriggerEvent(new HoverLostEvent());
		}
	}

	/// <summary>
	/// Executes a mouse move action.
	/// </summary>
	public static void ExecuteMousePositionChange(Vector2 newPosition)
	{
		if (_lastMousePosition == newPosition) return;

		_lastMousePosition = MousePosition;

		MousePosition = newPosition;

		PerformanceTrace.RunAndTrace(updateHoveredObjects, "Hover Update");
	}

	/// <summary>
	/// Executes a scroll action.
	/// </summary>
	public static void ExecuteScroll(float delta)
	{
		if (delta == 0) return;

		MouseWheelDelta += delta;

		foreach (var obj in GetNonPositionalInputQueue())
		{
			obj.TriggerEvent(new ScrollEvent(delta));
		}

		PerformanceTrace.RunAndTrace(updateHoveredObjects, "Hover Update");
	}

	/// <summary>
	/// Executes a pressed state change action on the specified button.
	/// </summary>
	public static void ExecuteMouseButtonStateChange(MouseButton button, bool pressed)
	{
		_mouseButtons[(int)button].SetState(pressed);

		if (pressed)
		{
			_clickDownGameObjects.Clear();

			foreach (var obj in GetPositionalInputQueue(MousePosition))
			{
				_clickDownGameObjects.Add(obj);
				if (obj.TriggerEvent(new MouseDownEvent(button, MousePosition)) == true) return;

				if (button == MouseButton.Left && obj.AcceptsFocus)
					ChangeFocus(obj);
			}

			if (FocusedObject is not null && _clickDownGameObjects.Contains(FocusedObject) == false)
				ChangeFocus(null);
		}
		else
		{
			propagatePositionalInputEvent(new MouseUpEvent(button, MousePosition));
			var clickUpGameObjects = GetPositionalInputQueue(MousePosition);

			foreach (var obj in clickUpGameObjects)
			{
				if (_clickDownGameObjects.Contains(obj))
				{
					if (obj.TriggerEvent(new ClickEvent(button, MousePosition))) break;
				}
			}
		}
	}

	#endregion

	#region Keyboard

	private static ButtonState[] _keyboardKeys = Array.Empty<ButtonState>();

	/// <summary>
	/// Returns the state of the specified key.
	/// </summary>
	public static ButtonState GetKey(Keys key) => GetKey((int)key);

	/// <summary>
	/// Event that get raised whenever a character is inputted on the keyboard.
	/// </summary>
	public static event Action<char>? OnTextInput;

	internal static ButtonState GetKey(int keycode)
	{
		if (keycode < (int)Keys.Amount)
			return _keyboardKeys[keycode];
		else
			return _keyboardKeys[(int)Keys.Unknown];
	}

	/// <summary>
	/// Executes a pressed state change action on the specified key.
	/// </summary>
	public static void ExecuteKeyboardKeyStateChange(Keys key, bool pressed)
	{
		_keyboardKeys[(int)key].SetState(pressed);

		if (pressed)
			propagateNonPositionalInputEvent(new KeyDownEvent(key));
		else
			propagateNonPositionalInputEvent(new KeyUpEvent(key));
	}

	/// <summary>
	/// Sets the repeat state of the specified key to true.
	/// </summary>
	public static void ExecuteKeyboardKeyRepeat(Keys key)
	{
		_keyboardKeys[(int)key].SetRepeat();

		propagateNonPositionalInputEvent(new KeyDownEvent(key, true));
	}

	/// <summary>
	/// Executes a text input action with the specified character.
	/// </summary>
	public static void ExecuteTextInput(char input)
	{
		OnTextInput?.Invoke(input);
	}

	#endregion

	#region Gamepad

	internal static List<IGamepad> _gamepads = new();
	public static int GetGamepadCount() => _gamepads.Count;
	public static IGamepad GetGamepad(int index) => _gamepads[index];

	internal static void HandleGamepadConnected(IGamepad gamepad)
		=> _gamepads.Add(gamepad);

	#endregion

	#region Other

	public static GameObject? FocusedObject { get; private set; }

	/// <summary>
	/// Changes the focused object. Can be set to null to remove current focus.
	/// </summary>
	public static bool ChangeFocus(GameObject? newFocus)
	{
		if (FocusedObject == newFocus)
			return true;

		var previousFocus = FocusedObject;
		FocusedObject = newFocus;

		if (previousFocus is not null)
		{
			previousFocus.HasFocus = false;
			previousFocus.TriggerEvent(new FocusLostEvent(newFocus));
		}

		if (FocusedObject is not null)
		{
			FocusedObject.HasFocus = true;
			FocusedObject.TriggerEvent(new FocusEvent(previousFocus));
		}

		return true;
	}

	/// <summary>
	/// Returns the movement vector by getting the state of the WASD and Arrow keys.
	/// </summary>
	/// <returns></returns>

	public static Vector2 GetDirectionalMovement()
	{
		var horizontal = 0;
		var vertical = 0;

		if (GetKey(Keys.W).Pressed || GetKey(Keys.Up).Pressed)
			vertical -= 1;

		if (GetKey(Keys.S).Pressed || GetKey(Keys.Down).Pressed)
			vertical += 1;

		if (GetKey(Keys.D).Pressed || GetKey(Keys.Right).Pressed)
			horizontal += 1;

		if (GetKey(Keys.A).Pressed || GetKey(Keys.Left).Pressed)
			horizontal -= 1;

		if (horizontal == 0 && vertical == 0)
			return Vector2.Zero;

		var direction = new Vector2(horizontal, vertical);
		return Vector2.Normalize(direction);
	}

	#endregion
}
