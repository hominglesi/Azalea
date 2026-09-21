using Azalea.Graphics;
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

	private Vector2 _mousePosition;

	public void Process(Channel<InputEvent> inputChannel)
	{
		while (inputChannel.Reader.TryRead(out var command))
		{
			switch (command)
			{
				case MouseMoveEvent(var position):
					_mousePosition = position;
					reprocessHoveredObjects(position);
					break;
				case MouseDownEvent(var _, var position):
					foreach (var obj in getPositionalInputQueue(position))
						if (obj.TriggerEvent(new ClickEvent(MouseButton.Left, position))) return;
					break;
				case KeyDownEvent or KeyUpEvent:
					foreach (var obj in getNonPositionalInputQueue())
						if (obj.TriggerEvent(command) == true) return;
					break;
				case CharInputEvent(var chr):
					Console.WriteLine(chr);
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
}
