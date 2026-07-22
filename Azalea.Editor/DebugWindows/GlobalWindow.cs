using Azalea.Editor.DebugWindows.Inspectors;
using Azalea.Editor.Design.Gui;
using Azalea.Platform;
using Azalea.Platform.Rendering;
using Azalea.Threading;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Azalea.Editor.DebugWindows;
internal static class GlobalWindow
{
	private static GUIWindow? _window;
	private static bool _shown = false;

	public static void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private static GUIGroup _applicationsGroup;
	private static Dictionary<Application, GUIGroup> _applicationGroups = [];

	private static GUILabel? _createdCommandsDisplay;
	private static Dictionary<Type, GUICounter> _commandDisplays = [];

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Global", new(100), new(400, 400));

			_window.AddGroup("Host");
			_window.AddButton("Create Application", () => GameHost.Main.CreateApplication());
			_applicationsGroup = _window.AddGroup("Applications");

			void createApplicationGroup(Application application)
			{
				_window!.SelectGroup(_applicationsGroup);
				var group = _window!.AddGroup("Application");
				_window.AddButton("Close", () => application.Close());

				_window.AddButton("Inspect PlatformWindow",
					() => PlatformWindowInspector.Create(application.Window, _window));
				_window.AddButton("Inspect PlatformRenderer",
					() => PlatformRendererInspector.Create(application.Renderer, _window));
				_window.AddButton("Inspect PlatformScheduler",
					() => PlatformSchedulerInspector.Create(application.Scheduler, _window));

				_window.FinishGroup();
				_window.FinishGroup();
				_applicationGroups.Add(application, group);
			}

			void removeApplicationGroup(Application application)
			{
				var group = _applicationGroups[application];
				_applicationGroups.Remove(application);
				_applicationsGroup.Remove(group);
			}

			foreach (var application in GameHost.Main.Applications)
				createApplicationGroup(application);

			_window.FinishGroup();

			GameHost.Main.Applications.OnItemAdded += applicaiton => Scheduler.Schedule(
				() => createApplicationGroup(applicaiton));
			GameHost.Main.Applications.OnItemRemoved += application => Scheduler.Schedule(
				() => removeApplicationGroup(application));

			_window.FinishGroup();

			_window.AddGroup("RenderCommands");
			_createdCommandsDisplay = _window.AddLabel("Created RenderCommands: " + RenderCommand.TotalCreated);
			_window.AddGroup("RenderCommand Types");

			foreach (var commandType in ReflectionUtils.GetAllChildrenOf(typeof(RenderCommand)))
			{
				var totalCreatedField = commandType.GetField("TotalCreated", BindingFlags.Static | BindingFlags.NonPublic)!;
				var commandLabel = _window.AddCounter($"{commandType.Name}: ", (int)totalCreatedField.GetValue(null)!);
				_commandDisplays.Add(commandType, commandLabel);
			}

			_window.FinishGroup();
			_window.FinishGroup();

			RenderCommand.OnCommandCreated += command =>
			{
				Scheduler.Schedule(() =>
				{
					_createdCommandsDisplay.Text = "Created Commands: " + RenderCommand.TotalCreated;
					_commandDisplays[command.GetType()].Value++;
				});
			};
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
