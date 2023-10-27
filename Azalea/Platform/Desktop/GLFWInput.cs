using Azalea.Graphics.GLFW;
using Azalea.Graphics.GLFW.Enums;
using Azalea.Inputs;

namespace Azalea.Platform.Desktop;
public class GLFWInput
{
	private GLFW_Window _window;

	public GLFWInput(GLFW_Window window)
	{
		_window = window;

		_keyCallback = onKeyEvent;
		GLFW.SetKeyCallback(_window, _keyCallback);
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

	public void Update()
	{
		Input.HandleMousePositionChange(GLFW.GetCursorPos(_window));
	}
}
