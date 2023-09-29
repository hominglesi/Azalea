using Azalea.Graphics.Veldrid;
using Azalea.Inputs;
using Veldrid;
using Veldrid.Sdl2;

namespace Azalea.Platform.Veldrid;

internal class VeldridInputManager
{
	private Sdl2Window _sdl;

	public VeldridInputManager(VeldridWindow window)
	{
		_sdl = window.Window;
		window.OnInput += handleInput;

		_sdl.MouseMove += processMouseMove;
		_sdl.MouseDown += processMouseDown;
		_sdl.MouseUp += processMouseUp;
		_sdl.MouseWheel += processMouseWheel;

		_sdl.KeyDown += processKeyDown;
		_sdl.KeyUp += processKeyUp;
	}

	private void handleInput()
	{
		var events = _sdl.PumpEvents();
		foreach (var charPress in events.KeyCharPresses)
		{
			Input.HandleTextInput(charPress);
		}
	}

	private void processKeyDown(KeyEvent obj)
	{
		if (obj.Repeat) return;

		var pressedKey = obj.Key.ToAzaleaKey();
		Input.HandleKeyboardKeyStateChange(pressedKey, true);
	}
	private void processKeyUp(KeyEvent obj)
	{
		var pressedKey = obj.Key.ToAzaleaKey();
		Input.HandleKeyboardKeyStateChange(pressedKey, false);
	}

	private void processMouseDown(MouseEvent obj)
	{
		var buttonIndex = (int)VeldridExtentions.ToAzaleaMouseInput(obj.MouseButton);
		if (buttonIndex > 4) return;

		Input.HandleMouseButtonStateChange((Inputs.MouseButton)buttonIndex, true);
	}

	private void processMouseUp(MouseEvent obj)
	{
		var buttonIndex = (int)VeldridExtentions.ToAzaleaMouseInput(obj.MouseButton);
		if (buttonIndex > 4) return;

		Input.HandleMouseButtonStateChange((Inputs.MouseButton)buttonIndex, false);
	}

	private void processMouseMove(MouseMoveEventArgs obj)
	{
		Input.HandleMousePositionChange(obj.MousePosition);
	}

	private void processMouseWheel(MouseWheelEventArgs args)
	{
		Input.HandleScroll(args.WheelDelta);
	}
}
