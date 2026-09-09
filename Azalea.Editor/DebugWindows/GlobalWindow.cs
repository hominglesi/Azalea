using Azalea.Editor.DebugWindows.Inspectors;
using Azalea.Editor.Design.Gui;
using Azalea.Platform;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Platform.Rendering.OpenGL.LoadingContext;
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

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Global", new(100), new(400, 400));

			_window.AddGroup("Host");
			_window.AddButton("Inspect PlatformAudio",
					() => PlatformAudioInspector.Create(GameHost.Main.Audio, _window));
			_window.AddButton("Create Application", () => GameHost.Main.CreateApplication());
			var applicationsGroup = _window.AddGroup("Applications");
			var applicationGroups = new Dictionary<Application, GUIGroup>();

			void createApplicationGroup(Application application)
			{
				_window!.SelectGroup(applicationsGroup);
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
				applicationGroups.Add(application, group);
			}

			void removeApplicationGroup(Application application)
			{
				var group = applicationGroups[application];
				applicationGroups.Remove(application);
				applicationsGroup.Remove(group);
			}

			foreach (var application in GameHost.Main.Applications)
				createApplicationGroup(application);

			_window.FinishGroup();

			GameHost.Main.Applications.OnItemAdded += applicaiton => Scheduler.Schedule(
				() => createApplicationGroup(applicaiton));
			GameHost.Main.Applications.OnItemRemoved += application => Scheduler.Schedule(
				() => removeApplicationGroup(application));

			_window.FinishGroup();
			_window.AddGroup("ThreadCommands");
			var otherCommandsCount = 0;
			var commandDisplays = new Dictionary<Type, GUICounter>();
			var totalCreatedCounter = _window.AddCounter("Total Created: ", ThreadCommand.TotalCreated);
			otherCommandsCount += ThreadCommand.TotalCreated;

			var renderCommands = _window.AddGroup("Render Commands");
			var renderCreatedCounter = _window.AddCounter("Total Created: ", RenderCommand.TotalCreated);
			otherCommandsCount -= RenderCommand.TotalCreated;
			_window.FinishGroup();

			var loadingCommands = _window.AddGroup("Loading Commands");
			var loadingCreatedCounter = _window.AddCounter("Total Created: ", LoadingCommand.TotalCreated);
			otherCommandsCount -= LoadingCommand.TotalCreated;
			_window.FinishGroup();

			var otherCommands = _window.AddGroup("Other Commands");
			var otherCreatedCounter = _window.AddCounter("Total Created: ", otherCommandsCount);
			_window.FinishGroup();

			foreach (var commandType in ReflectionUtils.GetAllChildrenOf(typeof(ThreadCommand)))
			{
				if (commandType.IsAbstract)
					continue;

				var totalCreatedField = commandType.GetField("TotalCreated", BindingFlags.Static | BindingFlags.NonPublic)!;

				if (commandType.IsSubclassOf(typeof(RenderCommand)))
					_window.SelectGroup(renderCommands);
				else if (commandType.IsSubclassOf(typeof(LoadingCommand)))
					_window.SelectGroup(loadingCommands);
				else
					_window.SelectGroup(otherCommands);

				var commandLabel = _window.AddCounter($"{commandType.Name}: ", (int)totalCreatedField.GetValue(null)!);
				commandDisplays.Add(commandType, commandLabel);

				_window.FinishGroup();
			}

			_window.FinishGroup();

			ThreadCommand.OnCommandCreated += command =>
			{
				totalCreatedCounter.Value = ThreadCommand.TotalCreated;
				renderCreatedCounter.Value = RenderCommand.TotalCreated;
				loadingCreatedCounter.Value = LoadingCommand.TotalCreated;
				otherCreatedCounter.Value = ThreadCommand.TotalCreated
					- RenderCommand.TotalCreated
					- LoadingCommand.TotalCreated;

				commandDisplays[command.GetType()].Value++;
			};

			if (GLRenderer.LoadingContext is not null)
				GLLoadingContextInspector.Inject(_window, GLRenderer.LoadingContext);
			else
				GLRenderer.LoadingContextCreated.OnValueChanged += _ => Scheduler.Schedule(
					() => GLLoadingContextInspector.Inject(_window, GLRenderer.LoadingContext!));
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
