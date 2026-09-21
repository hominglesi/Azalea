using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Camera;
using Azalea.Inputs;
using Azalea.Inputs.Events;
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

	public InputProcessor(GameObject root)
	{
		_root = root;
	}

	public event Action<char>? OnCharInput;

	private Vector2 _mousePosition;
	private readonly List<GameObject> _clickDownGameObjects = [];
	private GameObject? _focusedObject;

	public void Process(Channel<InputEvent> inputChannel)
	{
		while (inputChannel.Reader.TryRead(out var command))
		{
			switch (command)
			{
				case MouseMoveEvent(var position):
					_mousePosition = MainCamera.Instance.ToWorldSpace(position);
					reprocessHoveredObjects(_mousePosition);
					break;
				case MouseDownEvent(var button, var position):
					_clickDownGameObjects.Clear();

					bool focusAccepted = false;

					foreach (var obj in getPositionalInputQueue(position))
					{
						_clickDownGameObjects.Add(obj);

						if (focusAccepted == false && button == MouseButton.Left && obj.AcceptsFocus)
						{
							changeFocus(obj);
							focusAccepted = true;
						}

						if (obj.TriggerEvent(new MouseDownEvent(button, position)) == true) break;
					}

					if (_focusedObject is not null && focusAccepted == false)
						changeFocus(null);

					break;
				case MouseUpEvent(var button, var position):
					foreach (var obj in getNonPositionalInputQueue())
						if (obj.TriggerEvent(command) == true) return;

					ClickEvent? clickEvent = null;
					foreach (var obj in getPositionalInputQueue(position))
					{
						if (_clickDownGameObjects.Contains(obj))
						{
							clickEvent ??= new ClickEvent(button, position);
							if (obj.TriggerEvent(clickEvent)) break;
						}
					}

					break;
				case KeyDownEvent or KeyUpEvent:
					foreach (var obj in getNonPositionalInputQueue())
						if (obj.TriggerEvent(command) == true) return;
					break;
				case CharInputEvent(var chr):
					OnCharInput?.Invoke(chr);
					break;
				case ScrollEvent(var delta):
					foreach (var obj in getNonPositionalInputQueue())
						obj.TriggerEvent(command);

					// For now we recalculate hovered objects since a scroll
					// often moves the objects. We should recalculate when objects are moved instead.
					reprocessHoveredObjects(_mousePosition);
					break;
			}
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

	
	private readonly List<GameObject> _hoveredObjects = [];
	private readonly List<GameObject> _lastHoveredObjects = [];
	private GameObject? _hoverHandledObject;

	private void reprocessHoveredObjects(Vector2 newPosition)
	{
		GameObject? lastHoverHandledObject = _hoverHandledObject;
		_hoverHandledObject = null;

		_lastHoveredObjects.Clear();
		_lastHoveredObjects.AddRange(_hoveredObjects);

		_hoveredObjects.Clear();

		var positionalQueue = getPositionalInputQueue(newPosition);

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

	private bool changeFocus(GameObject? newFocus)
	{
		if (_focusedObject == newFocus)
			return true;

		var previousFocus = _focusedObject;
		_focusedObject = newFocus;

		if (previousFocus is not null)
		{
			previousFocus.HasFocus = false;
			previousFocus.TriggerEvent(new FocusLostEvent(newFocus));
		}

		if (_focusedObject is not null)
		{
			_focusedObject.HasFocus = true;
			_focusedObject.TriggerEvent(new FocusEvent(previousFocus));
		}

		return true;
	}
}
