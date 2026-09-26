using Azalea.Editor.Design.Gui;
using Azalea.Graphics.Camera;
using Azalea.Inputs;
using Azalea.Platform;

namespace Azalea.Editor.DebugWindows;
internal class CameraWindow(AzaleaGame game)
{
	private AzaleaGame _game = game;
	private GUIWindow? _window;
	private bool _shown = false;

	public void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create(_game.App, "Camera", new(100), new(400, 400));
			_window.AddSliderFloat("X Position", minValue: -1000, maxValue: 1000, initialValue: 0, continuous: false)
				.OnValueChanged(x => MainCamera.Instance.Position = new(x, MainCamera.Instance.Position.Y));
			_window.AddSliderFloat("Y Position", minValue: -1000, maxValue: 1000, initialValue: 0, continuous: false)
				.OnValueChanged(y => MainCamera.Instance.Position = new(MainCamera.Instance.Position.X, y));
			_window.AddSliderFloat("Zoom", minValue: 0.5f, maxValue: 1.5f, initialValue: 1, continuous: false)
				.OnValueChanged(zoom => MainCamera.Instance.Zoom = zoom);
			_window.AddLabel(() => $"Mouse Position: {_window.App.Input.State.MousePosition}");
		}

		_window.Show();
	}

	private void hide()
	{
		if (_window is null)
			return;

		_window.Hide();
	}
}
