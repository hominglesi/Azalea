using Azalea.Graphics.GLFW;
using Azalea.Graphics.GLFW.Enums;
using Azalea.Inputs;
using System.Collections.Generic;

namespace Azalea.Platform.Desktop;
public class GLFWInput
{
	private GLFW_Window _window;

	private List<GLFWJoystick> _joysticks = new();

	public GLFWInput(GLFW_Window window)
	{
		_window = window;

		_keyCallback = onKeyEvent;
		GLFW.SetKeyCallback(_window, _keyCallback);

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

	private GLFW.KeyCallback _keyCallback;
	private void onKeyEvent(GLFW_Window window, int key, int scancode, int action, int mods)
	{
		Keys azkey = GLFWExtentions.KeycodeToKey(key);

		if (action == (int)GLFWKeyEvent.Press || action == (int)GLFWKeyEvent.Release)
		{
			Input.HandleKeyboardKeyStateChange(azkey, action == (int)GLFWKeyEvent.Press);
		}
		else if (action == (int)GLFWKeyEvent.Repeat)
		{
			Input.HandleKeyboardKeyRepeat(azkey);
		}
	}

	private GLFW.MouseButtonCallback _mouseButtonCallback;
	private void onMouseButtonEvent(GLFW_Window window, int button, int action, int mods)
	{
		Input.HandleMouseButtonStateChange((MouseButton)button, action == (int)GLFWKeyEvent.Press);
	}

	private GLFW.ScrollCallback _scrollCallback;
	private void onScrollEvent(GLFW_Window window, double xOffset, double yOffset)
	{
		_xScroll += (float)xOffset;
		_yScroll += (float)yOffset;
	}
	private float _xScroll;
	private float _yScroll;

	public void Update()
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
}
