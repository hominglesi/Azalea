using Azalea.Design.Containers;
using Azalea.Graphics;
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

	private static Composition? _rootObject;

	internal static List<GameObject> PositionalInputQueue => buildPositionalInputQueue(_mousePosition);
	internal static List<GameObject> NonPositionalInputQueue => buildNonPositionalInputQueue();

	internal static void Initialize(Composition rootObject)
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

	private static GameObject? _hoverHandledObject;
	private static void updateHoverEvents()
	{
		GameObject? lastHoverHandledObject = _hoverHandledObject;
		_hoverHandledObject = null;

		_lastHoveredObjects.Clear();
		_lastHoveredObjects.AddRange(_hoveredObjects);

		_hoveredObjects.Clear();

		foreach (var obj in PositionalInputQueue)
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

	private static readonly List<GameObject> _clickDownGameObjects = new();
	internal static void HandleMouseButtonStateChange(MouseButton button, bool pressed)
	{
		_mouseButtons[(int)button].SetState(pressed);

		if (pressed)
		{
			_clickDownGameObjects.Clear();

			foreach (var obj in PositionalInputQueue)
			{
				_clickDownGameObjects.Add(obj);
				if (obj.TriggerEvent(new MouseDownEvent(button, _mousePosition)) == true) return;

				if (button == MouseButton.Left && obj.AcceptsFocus)
					ChangeFocus(obj);
			}

			if (FocusedObject is not null && _clickDownGameObjects.Contains(FocusedObject) == false)
				ChangeFocus(null);
		}
		else
		{
			propagatePositionalInputEvent(new MouseUpEvent(button, _mousePosition));
			var clickUpGameObjects = PositionalInputQueue;

			foreach (var obj in clickUpGameObjects)
			{
				if (_clickDownGameObjects.Contains(obj))
				{
					if (obj.TriggerEvent(new ClickEvent(button, _mousePosition))) break;
				}
			}
		}
	}

	public static GameObject? FocusedObject;

	public static bool ChangeFocus(GameObject? potentialFocusTarget)
	{
		if (FocusedObject == potentialFocusTarget)
			return true;

		var previousFocus = FocusedObject;
		FocusedObject = potentialFocusTarget;

		if (previousFocus is not null)
		{
			previousFocus.HasFocus = false;
			previousFocus.TriggerEvent(new FocusLostEvent(potentialFocusTarget));
		}

		if (FocusedObject is not null)
		{
			FocusedObject.HasFocus = true;
			FocusedObject.TriggerEvent(new FocusEvent(previousFocus));
		}

		return true;
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
