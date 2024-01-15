using Azalea.Inputs;
using Azalea.Platform.Glfw;
using System.Collections.Generic;

namespace Azalea.Platform.Desktop;
internal class GLFWInput : IInputManager
{
	private Window _window;

	private List<GLFWJoystick> _joysticks = new();

	public GLFWInput(Window window)
	{
		_window = window;

		_keyCallback = onKeyEvent;
		GLFW.SetKeyCallback(_window, _keyCallback);

		_charCallback = onCharEvent;
		GLFW.SetCharCallback(_window, _charCallback);

		_mouseButtonCallback = onMouseButtonEvent;
		GLFW.SetMouseButtonCallback(_window, _mouseButtonCallback);

		_scrollCallback = onScrollEvent;
		GLFW.SetScrollCallback(_window, _scrollCallback);

		for (int i = 0; i < Input._joystickSlots; i++)
		{
			if (GLFW.JoystickPresent(i))
			{
				var name = GLFW.GetJoystickName(i);
				var joystick = new GLFWJoystick(i, name);
				_joysticks.Add(joystick);

				Input._joysticks[i] = joystick;
			}
		}
	}

	public void ProcessInputs()
	{
		Input.HandleMousePositionChange(GLFW.GetCursorPos(_window));

		Input.HandleScroll(_yScroll);
		_xScroll = 0;
		_yScroll = 0;

		foreach (var joystick in _joysticks)
		{
			var axies = GLFW.GetJoystickAxes(joystick.Handle);
			joystick.SetAxies(axies);
		}
	}

	private GLFW.KeyCallback _keyCallback;
	private void onKeyEvent(Window window, int key, int scancode, int action, int mods)
	{
		Keys azkey = GLFWExtentions.KeycodeToKey(key);

		if (action == (int)KeyEvent.Press || action == (int)KeyEvent.Release)
		{
			Input.HandleKeyboardKeyStateChange(azkey, action == (int)KeyEvent.Press);
		}
		else if (action == (int)KeyEvent.Repeat)
		{
			Input.HandleKeyboardKeyRepeat(azkey);
		}
	}

	private GLFW.CharCallback _charCallback;
	private void onCharEvent(Window window, uint charCode)
	{
		Input.HandleTextInput((char)charCode);
	}

	private GLFW.MouseButtonCallback _mouseButtonCallback;
	private void onMouseButtonEvent(Window window, int button, int action, int mods)
	{
		Input.HandleMouseButtonStateChange((MouseButton)button, action == (int)KeyEvent.Press);
	}

	private GLFW.ScrollCallback _scrollCallback;
	private void onScrollEvent(Window window, double xOffset, double yOffset)
	{
		_xScroll += (float)xOffset;
		_yScroll += (float)yOffset;
	}
	private float _xScroll;
	private float _yScroll;
}
