using Azalea.Graphics.Containers;
using Azalea.Inputs.Events;
using System;

namespace Azalea.Graphics.UserInterface;

public class Button : Container
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

	protected override bool OnClick(ClickEvent e)
	{
		Action?.Invoke();
		return true;
	}
}
