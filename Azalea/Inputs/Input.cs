using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Inputs.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Inputs;

public static class Input
{
	#region Public facing

	public static Vector2 MousePosition => _mousePosition;

	public static ButtonState GetKey(Keys key) => GetKey((int)key);
	public static ButtonState GetKey(int keycode) => _keyboardKeys[keycode];
	public static ButtonState GetMouseButton(MouseButton button) => _mouseButtons[(int)button];

	public static event Action<char>? OnTextInput;

	public static IReadOnlyList<GameObject> GetHoveredObjects() => _hoveredObjects;

	#endregion

	internal static ButtonState[] _mouseButtons = new ButtonState[1];
	internal static ButtonState[] _keyboardKeys = new ButtonState[1];

	private static Vector2 _lastMousePosition = Vector2.Zero;
	private static Vector2 _mousePosition = Vector2.Zero;

	private static Container? _rootObject;

	internal static List<GameObject> PositionalInputQueue => buildPositionalInputQueue(_mousePosition);
	internal static List<GameObject> NonPositionalInputQueue => buildNonPositionalInputQueue();

	internal static void Initialize(Container rootObject)
	{
		_rootObject = rootObject;

		var mouseButtonCount = (int)MouseButton.LastButton;
		_mouseButtons = new ButtonState[mouseButtonCount];

		for (int i = 0; i < mouseButtonCount; i++)
		{
			_mouseButtons[i] = new ButtonState(i);
		}

		var keyButtonCount = (int)Keys.LastKey;
		_keyboardKeys = new ButtonState[keyButtonCount];

		for (int i = 0; i < keyButtonCount; i++)
		{
			_keyboardKeys[i] = new ButtonState(i);

			_keyboardKeys[i].OnRepeat += HandleKeyboardKeyRepeat;
		}
	}

	internal static void LateUpdate()
	{
		foreach (var key in _keyboardKeys)
		{
			key.Update();
		}

		foreach (var mouseButton in _mouseButtons)
		{
			mouseButton.Update();
		}
	}

	internal static void HandleMousePositionChange(Vector2 newPosition)
	{
		if (_lastMousePosition == newPosition) return;

		_lastMousePosition = _mousePosition;

		_mousePosition = newPosition;

		updateHoverEvents();
	}

	private static readonly List<GameObject> _lastHoveredObjects = new();
	private static readonly List<GameObject> _hoveredObjects = new();
	private static void updateHoverEvents()
	{
		_lastHoveredObjects.Clear();
		_lastHoveredObjects.AddRange(_hoveredObjects);

		_hoveredObjects.Clear();

		foreach (var obj in PositionalInputQueue)
		{
			_hoveredObjects.Add(obj);
			_lastHoveredObjects.Remove(obj);

			if (obj.Hovered)
			{
				continue;
			}

			obj.Hovered = true;
			obj.TriggerEvent(new HoverEvent());
		}

		foreach (var obj in _lastHoveredObjects)
		{
			obj.Hovered = false;
			obj.TriggerEvent(new HoverLostEvent());
		}
	}

	internal static void HandleMouseButtonStateChange(MouseButton button, bool pressed)
	{
		_mouseButtons[(int)button].SetState(pressed);

		if (pressed)
			propagatePositionalInputEvent(new MouseDownEvent(button, _mousePosition));
		else
			propagatePositionalInputEvent(new MouseUpEvent(button, _mousePosition));
	}

	internal static void HandleKeyboardKeyStateChange(Keys key, bool pressed)
	{
		_keyboardKeys[(int)key].SetState(pressed);

		if (pressed)
			propagateInputEvent(new KeyDownEvent(key));
		else
			propagateInputEvent(new KeyUpEvent(key));
	}

	internal static void HandleKeyboardKeyRepeat(int keyCode)
	{
		propagateInputEvent(new KeyDownEvent((Keys)keyCode, true));
	}

	internal static void HandleTextInput(char input)
	{
		OnTextInput?.Invoke(input);
	}

	private static List<GameObject> buildNonPositionalInputQueue()
	{
		Debug.Assert(_rootObject is not null);

		var inputQueue = new List<GameObject>();
		_rootObject.BuildNonPositionalInputQueue(inputQueue);
		inputQueue.Reverse();
		return inputQueue;
	}

	private static List<GameObject> buildPositionalInputQueue(Vector2 position)
	{
		Debug.Assert(_rootObject is not null);

		var inputQueue = new List<GameObject>();
		_rootObject.BuildPositionalInputQueue(position, inputQueue);
		inputQueue.Reverse();
		return inputQueue;
	}

	private static void propagateInputEvent(InputEvent e)
	{
		foreach (var obj in NonPositionalInputQueue)
		{
			if (obj.TriggerEvent(e) == true) return;
		}
	}

	private static void propagatePositionalInputEvent(InputEvent e)
	{
		foreach (var obj in PositionalInputQueue)
		{
			if (obj.TriggerEvent(e) == true) return;
		}
	}
}
