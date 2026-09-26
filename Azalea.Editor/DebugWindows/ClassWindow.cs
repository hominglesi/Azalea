using Azalea.Editor.Design.Gui;
using System;
using System.Linq;

namespace Azalea.Editor.DebugWindows;
internal class ClassWindow(AzaleaGame game)
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
			_window = GUIWindow.Create(_game.App, "ClassTree", new(100), new(400, 400));

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.FullName!.StartsWith("System")
					|| assembly.FullName!.StartsWith("Microsoft.")
					|| assembly.FullName!.StartsWith("netstandard"))
					continue;

				_window.AddGroup(assembly.FullName[..assembly.FullName.IndexOf(',')]);

				Console.WriteLine(assembly.FullName);
				foreach (var item in assembly.GetTypes()
					.Where(t => t.IsClass))
				{
					if (item.FullName!.Contains('>'))
						continue;

					_window.AddLabel(item.FullName!);
				}

				_window.FinishGroup();
			}
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
