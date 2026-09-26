using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Camera;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Inputs.Gamepads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Channels;

namespace Azalea.Platform.Scheduling;

public class InputProcessor
{
	private readonly GameObject _root;
	private readonly Action<bool>? _showDroppableCursorChanged;
	private readonly IGamepadManager _gamepadInput;
	public readonly InputState State;

	internal InputProcessor(GameObject root, IGamepadManager gamepadInput, Action<bool>? showDroppableCursorChanged = null)
	{
		_root = root;
		_showDroppableCursorChanged = showDroppableCursorChanged;
		_gamepadInput = gamepadInput;
		State = new InputState();
	}

	private readonly List<GameObject> _clickDownGameObjects = [];

	public void Process(Channel<InputEvent> inputChannel)
	{
		while (inputChannel.Reader.TryRead(out var command))
			handleEvent(command);
	}

	private void handleEvent(InputEvent e)
	{
		e.State = State;

		switch (e)
		{
			case MouseMoveEvent(var position):
				State.MousePosition = MainCamera.Instance.ToWorldSpace(position);
				reprocessHoveredObjects(State.MousePosition);
				break;
			case MouseDownEvent(var button, var position):
				State.SetMouseButtonPressed(button, true);
				_clickDownGameObjects.Clear();

				bool focusAccepted = false;

				foreach (var obj in getPositionalInputQueue(position))
				{
					_clickDownGameObjects.Add(obj);

					if (focusAccepted == false && button == MouseButton.Left && obj.AcceptsFocus)
					{
						ChangeFocus(obj);
						focusAccepted = true;
					}

					if (obj.TriggerEvent(e) == true) break;
				}

				if (State.FocusedObject is not null && focusAccepted == false)
					ChangeFocus(null);

				break;
			case MouseUpEvent(var button, var position):
				State.SetMouseButtonPressed(button, false);
				foreach (var obj in getNonPositionalInputQueue())
					if (obj.TriggerEvent(e) == true) break;

				ClickEvent? clickEvent = null;
				foreach (var obj in getPositionalInputQueue(position))
				{
					if (_clickDownGameObjects.Contains(obj))
					{
						clickEvent ??= new ClickEvent(button, position);
						clickEvent.State = State;
						if (obj.TriggerEvent(clickEvent)) break;
					}
				}

				break;
			case KeyDownEvent(var key, var isRepeat):
				State.SetKeyPressed(key, true);
				foreach (var obj in getNonPositionalInputQueue())
					if (obj.TriggerEvent(e) == true) break;
				break;
			case KeyUpEvent(var key):
				State.SetKeyPressed(key, false);
				foreach (var obj in getNonPositionalInputQueue())
					if (obj.TriggerEvent(e) == true) break;
				break;
			case CharInputEvent(var chr):
				State.TriggerCharInput(chr);
				break;
			case ScrollEvent:
				foreach (var obj in getNonPositionalInputQueue())
					obj.TriggerEvent(e);

				// For now we recalculate hovered objects since a scroll
				// often moves the objects. We should recalculate when objects are moved instead.
				reprocessHoveredObjects(State.MousePosition);
				break;
			case FileDroppedEvent:
				foreach (var obj in getPositionalInputQueue(State.MousePosition))
					if (obj.TriggerEvent(e)) break;
				break;
		}
	}

	private IReadOnlyList<GameObject> getNonPositionalInputQueue()
	{
		var inputQueue = new List<GameObject>();
		_root.BuildNonPositionalInputQueue(inputQueue);
		inputQueue.Reverse();

		return inputQueue;
	}

	private IReadOnlyList<GameObject> getPositionalInputQueue(Vector2 position)
	{
		var inputQueue = new List<GameObject>();
		_root.BuildPositionalInputQueue(position, inputQueue);
		inputQueue.Reverse();

		return inputQueue;
	}

	private readonly List<GameObject> _lastHoveredObjects = [];
	private GameObject? _hoverHandledObject;

	private void reprocessHoveredObjects(Vector2 newPosition)
	{
		GameObject? lastHoverHandledObject = _hoverHandledObject;
		var hoveredObjects = State.HoveredObjectsInternal;

		_hoverHandledObject = null;

		_lastHoveredObjects.Clear();
		_lastHoveredObjects.AddRange(hoveredObjects);

		hoveredObjects.Clear();

		var showDroppableCursor = false;
		_showDroppableCursorChanged?.Invoke(showDroppableCursor);

		var positionalQueue = getPositionalInputQueue(newPosition);

		foreach (var obj in positionalQueue)
		{
			hoveredObjects.Add(obj);
			_lastHoveredObjects.Remove(obj);

			if (showDroppableCursor == false && obj.AcceptsFiles)
			{
				showDroppableCursor = true;
				_showDroppableCursorChanged?.Invoke(showDroppableCursor);
			}

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

	public bool ChangeFocus(GameObject? newFocus)
	{
		if (State.FocusedObject == newFocus)
			return true;

		var previousFocus = State.FocusedObject;
		State.FocusedObject = newFocus;

		if (previousFocus is not null)
		{
			previousFocus.HasFocus = false;
			previousFocus.TriggerEvent(new FocusLostEvent(newFocus));
		}

		if (State.FocusedObject is not null)
		{
			State.FocusedObject.HasFocus = true;
			State.FocusedObject.TriggerEvent(new FocusEvent(previousFocus));
		}

		return true;
	}

	public IGamepad? GetGamepad(int index) => _gamepadInput!.GetGamepad(index);

	#region Simulations

	public void SimulateCharInput(string charString)
	{
		foreach (var chr in charString)
			handleEvent(new CharInputEvent(chr));
	}

	public void SimulateKeyInput(Keys key, int count = 1)
	{
		for (int i = 0; i < count; i++)
		{
			handleEvent(new KeyDownEvent(key, false));
			handleEvent(new KeyUpEvent(key));
		}
	}

	#endregion
}
