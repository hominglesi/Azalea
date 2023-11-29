using Azalea.Design.Containers;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System;

namespace Azalea.Design.UserInterface;

public class Button : Composition
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

	public void SimulateClick()
	{
		OnClick(new ClickEvent(MouseButton.Left, Position));
	}
}
