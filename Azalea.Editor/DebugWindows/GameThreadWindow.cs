using Azalea.Editor.Design.Gui;
using Azalea.Threading;
using System.Collections.Generic;

namespace Azalea.Editor.DebugWindows;
internal class GameThreadWindow
{
	private static GUIWindow? _window;
	private static bool _shown = false;

	public static void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private static readonly Dictionary<GameThread, GUIGroup> _gameThreadGroups = [];

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Game Threads", new(100), new(400, 400));
			foreach (var thread in GameThread.ActiveThreads)
				addThreadGroup(thread);

			GameThread.OnThreadStarted += addThreadGroup;
			GameThread.OnThreadStopped += removeThreadGroup;
		}

		_window.Show();

		void addThreadGroup(GameThread thread)
		{
			var group = _window!.AddGroup(thread.DisplayName);
			_window.AddLabel($"Target Interval: {thread.TargetInterval}ms");
			_window.FinishGroup();
			_gameThreadGroups.Add(thread, group);
		}

		void removeThreadGroup(GameThread thread)
		{
			_window!.RemoveElement(_gameThreadGroups[thread]);
			_gameThreadGroups.Remove(thread);
		}
	}

	private static void hide()
	{
		if (_window is null)
			return;

		_window.Hide();
	}
}
