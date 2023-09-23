using Azalea.Inputs.Events;
using System;

namespace Azalea.Graphics.Containers;

public class ClickableContainer : Container
{
	private Action? _action;

	public Action? Action
	{
		get => _action;
		set
		{
			_action = value;
		}
	}

	protected override bool OnMouseDown(MouseDownEvent e)
	{
		if (_action is not null)
			Action?.Invoke();
		return true;
	}
}
