using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Inputs.Gamepads;
using Azalea.Scenes;
using Azalea.Utils.Proxies;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Channels;

namespace Azalea.Platform.Scheduling;

public class InputProcessor
{
	private readonly IProxy<bool> _droppableCursorProxy;
	private bool _droppableCursor;
	private readonly IGamepadManager _gamepadInput;
	public readonly InputState State;
	public IScene? Scene { get; internal set; }

	internal InputProcessor(IGamepadManager gamepadInput, IProxy<bool> droppableCursorProxy)
	{
		_droppableCursorProxy = droppableCursorProxy;
		_gamepadInput = gamepadInput;
		State = new InputState();

		_droppableCursorProxy.HasNewValue(out _droppableCursor);
	}

	private readonly List<ISceneObject> _clickDownGameObjects = [];

	public void Process(Channel<InputEvent> inputChannel)
	{
		while (inputChannel.Reader.TryRead(out var command))
			handleEvent(command);
	}

	private void handleEvent(InputEvent e)
	{
		if (Scene is null) return;

		e.State = State;

		switch (e)
		{
			case MouseMoveEvent(var position):
				State.MousePosition = position;
				reprocessHoveredObjects(State.MousePosition);
				break;
			case MouseDownEvent(var button, var position):
				State.SetMouseButtonPressed(button, true);
				_clickDownGameObjects.Clear();

				bool focusAccepted = false;

				foreach (var obj in Scene.BuildPositionalInputQueue(position))
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

				ClickEvent? clickEvent = null;
				foreach (var obj in Scene.BuildPositionalInputQueue(position))
				{
					if (_clickDownGameObjects.Contains(obj))
					{
						clickEvent ??= new ClickEvent(button, position);
						clickEvent.State = State;
						if (obj.TriggerEvent(clickEvent)) break;
					}
				}

				foreach (var obj in Scene.BuildNonPositionalInputQueue())
					obj.TriggerEvent(e);

				break;
			case KeyDownEvent(var key, var _):
				State.SetKeyPressed(key, true);
				foreach (var obj in Scene.BuildNonPositionalInputQueue())
					if (obj.TriggerEvent(e) == true) break;
				break;
			case KeyUpEvent(var key):
				State.SetKeyPressed(key, false);
				foreach (var obj in Scene.BuildNonPositionalInputQueue())
					if (obj.TriggerEvent(e) == true) break;
				break;
			case CharInputEvent(var chr):
				State.TriggerCharInput(chr);
				break;
			case ScrollEvent:
				foreach (var obj in Scene.BuildNonPositionalInputQueue())
					obj.TriggerEvent(e);

				// For now we recalculate hovered objects since a scroll
				// often moves the objects. We should recalculate when objects are moved instead.
				reprocessHoveredObjects(State.MousePosition);
				break;
			case FileDroppedEvent:
				foreach (var obj in Scene.BuildPositionalInputQueue(State.MousePosition))
					if (obj.TriggerEvent(e)) break;
				break;
		}
	}

	private readonly List<ISceneObject> _lastHoveredObjects = [];
	private ISceneObject? _hoverHandledObject;

	private void reprocessHoveredObjects(Vector2 newPosition)
	{
		ISceneObject? lastHoverHandledObject = _hoverHandledObject;
		var hoveredObjects = State.HoveredObjectsInternal;

		_hoverHandledObject = null;

		_lastHoveredObjects.Clear();
		_lastHoveredObjects.AddRange(hoveredObjects);

		hoveredObjects.Clear();

		var showDroppableCursor = false;

		Debug.Assert(Scene is not null);
		var positionalQueue = Scene.BuildPositionalInputQueue(newPosition);

		foreach (var obj in positionalQueue)
		{
			hoveredObjects.Add(obj);
			_lastHoveredObjects.Remove(obj);

			if (showDroppableCursor == false && obj.AcceptsFiles)
			{
				showDroppableCursor = true;
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

		if (_droppableCursor != showDroppableCursor)
		{
			_droppableCursor = showDroppableCursor;
			_droppableCursorProxy.SetValue(_droppableCursor);
		}
	}

	public bool ChangeFocus(ISceneObject? newFocus)
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
