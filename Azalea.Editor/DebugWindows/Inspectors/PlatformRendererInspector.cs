using Azalea.Editor.Design.Gui;
using Azalea.Extentions;
using Azalea.Platform;
using Azalea.Platform.Rendering;
using Azalea.Threading;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformRendererInspector
{
	public static GUIWindow Create(Application app, PlatformRenderer renderer, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create(app, "PlatformRenderer", position, new(400, 400));

		window.AddPropertyLabel("Staged Queue Overrides", renderer.CreateProxy<int>("StagedQueueOverrides"));
		window.AddPropertyLabel("No Staged Queue Frames", renderer.Thread.CreateProxy<int>("NoStagedQueueFrames"));

		GUIGroup? commandsGroup = null;
		window.AddButton("Snapshot Frame Commands", () =>
		{
			renderer.Thread.SnapshotNextFrame = true;
		});
		renderer.Thread.CommandSnapshotCreated += commands => Scheduler.Schedule(
			() =>
			{
				commandsGroup!.Clear();
				window.SelectGroup(commandsGroup);
				foreach (var command in commands)
					window.AddLabel(command);
				window.FinishGroup();
			});

		commandsGroup = window.AddGroup("Frame Commands Snapshot");
		window.AddLabel("Take first snapshot to see commands!");
		window.FinishGroup();

		window.AddPropertyLabel("Stopped", renderer.CreateProxy<bool>("Stopped"));
		window.AddButton("Stop", renderer.Stop);

		var thread = renderer.Thread;
		window.AddGroup("Render Thread");
		GameThreadInspector.Inject(window, thread);
		window.FinishGroup();

		return window;
	}
}
