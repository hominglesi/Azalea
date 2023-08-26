using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Inputs.Handlers;
using Azalea.Inputs.States;
using Azalea.Platform;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Azalea.Inputs;

public abstract class InputManager : Container
{
	private readonly Dictionary<MouseButton, MouseButtonEventManager> mouseButtonEventManagers = new();

	public readonly InputState CurrentState;

	protected GameHost Host => AzaleaGame.Main.Host;

	/// <summary>
	/// The currently focused <see cref="GameObject"/>. Null if there is no current focus.
	/// </summary>
	public GameObject? FocusedObject { get; internal set; }

	protected abstract ImmutableArray<InputHandler> InputHandlers { get; }

	protected InputManager()
	{
		CurrentState = new InputState(null);

		foreach (var button in Enum.GetValues<MouseButton>())
		{
			var manager = CreateButtonEventManagerFor(button);
			mouseButtonEventManagers.Add(button, manager);
		}
	}

	protected virtual MouseButtonEventManager CreateButtonEventManagerFor(MouseButton button)
		=> button switch
		{
			MouseButton.Left => new MouseLeftButtonEventManager(button),
			_ => new MouseMinorButtonEventManager(button)
		};

	protected override void Update()
	{
		base.Update();
	}

	public bool ChangeFocus(GameObject? newTarget) => ChangeFocus(newTarget, CurrentState);
	protected bool ChangeFocus(GameObject? newTarget, InputState state)
	{
		if (newTarget == FocusedObject) return true;

		if (newTarget is not null && newTarget.AcceptsFocus == false) return false;

		if (FocusedObject is not null && FocusedObject.AcceptsFocus)
		{
			FocusedObject.HasFocus = false;
		}

		FocusedObject = newTarget;

		if (FocusedObject is not null)
		{
			FocusedObject.HasFocus = true;
		}

		return true;
	}

	private class MouseLeftButtonEventManager : MouseButtonEventManager
	{
		public MouseLeftButtonEventManager(MouseButton button)
			: base(button) { }
	}

	private class MouseMinorButtonEventManager : MouseButtonEventManager
	{
		public MouseMinorButtonEventManager(MouseButton button)
			: base(button) { }
	}
}
