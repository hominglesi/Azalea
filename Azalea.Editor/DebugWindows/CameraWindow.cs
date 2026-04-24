using Azalea.Editor.Design.Gui;
using Azalea.Graphics.Camera;
using Azalea.Inputs;

namespace Azalea.Editor.DebugWindows;
internal static class CameraWindow
{
	private static GUIWindow? _window;
	private static bool _shown = false;

	public static void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Camera", new(400, 400));
			_window.AddSliderFloat("X Position", minValue: -1000, maxValue: 1000, initialValue: 0, continuous: false)
				.OnValueChanged(x => MainCamera.Instance.Position = new(x, MainCamera.Instance.Position.Y));
			_window.AddSliderFloat("Y Position", minValue: -1000, maxValue: 1000, initialValue: 0, continuous: false)
				.OnValueChanged(y => MainCamera.Instance.Position = new(MainCamera.Instance.Position.X, y));
			_window.AddSliderFloat("Zoom", minValue: 0.5f, maxValue: 1.5f, initialValue: 1, continuous: false)
				.OnValueChanged(zoom => MainCamera.Instance.Zoom = zoom);
			_window.AddLabel(() => $"Mouse Position: {Input.MousePosition}");
		}

		_window.Show();
	}

	private static void hide()
	{
		if (_window is null)
			return;

		_window.Hide();
	}
}
