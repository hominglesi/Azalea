using Azalea.Design.Containers;
using Azalea.Editing;
using Azalea.Graphics;
using Azalea.Graphics.Camera;
using Azalea.Inputs.Events;
using Azalea.Inputs.Gamepads;
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
		foreach (var key in _keyboardKeys)
			key.Update();

		foreach (var mouseButton in _mouseButtons)
			mouseButton.Update();
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

	#endregion

	#region Gamepad

	private static IGamepadManager? _gamepadManager;

	internal static void SetGamepadManager(IGamepadManager gamepadManager)
		=> _gamepadManager = gamepadManager;

	public static IGamepad? GetGamepad(int index)
		=> _gamepadManager!.GetGamepad(index);

	#endregion

	#region Files

	public static void ExecuteFileDropped(string[] filePaths)
	{
		var e = new FileDroppedEvent(filePaths);

		foreach (var obj in _hoveredObjects)
		{
			if (obj.TriggerEvent(e))
				break;
		}
	}

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

	#endregion
}
