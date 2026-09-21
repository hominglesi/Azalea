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

	public void Process(Channel<InputEvent> inputChannel)
	{
		while (inputChannel.Reader.TryRead(out var command))
		{
			switch (command)
			{
				case KeyDownEvent or KeyUpEvent:
					foreach (var obj in getNonPositionalInputQueue())
						if (obj.TriggerEvent(command) == true) return;
					break;
				case MouseDownEvent(var _, var position):
					foreach (var obj in getPositionalInputQueue(position))
						if (obj.TriggerEvent(new ClickEvent(MouseButton.Left, position))) return;
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
}
