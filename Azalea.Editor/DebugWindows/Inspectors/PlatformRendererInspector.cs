using Azalea.Editor.Design.Gui;
using Azalea.Platform.Rendering;
using Azalea.Threading;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformRendererInspector
{
	public static GUIWindow Create(PlatformRenderer renderer, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create("PlatformRenderer", position, new(400, 400));

		window.AddObservingLabel("Staged Queue Overrides", renderer.StagedQueueOverrides);
		window.AddObservingLabel("No Staged Queue Frames", renderer.NoStagedQueueFrames);

		GUIGroup? commandsGroup = null;
		window.AddButton("Snapshot Frame Commands", () =>
		{

			renderer.SnapshotNextFrame = true;
		});
		renderer.CommandSnapshotCreated += commands => Scheduler.Schedule(
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

		window.AddObservingLabel("Stopped", renderer.Stopped);
		window.AddButton("Stop", renderer.Stop);

		var thread = renderer.Thread;
		window.AddGroup("Render Thread");
		GameThreadInspector.Inject(window, thread);
		window.FinishGroup();

		return window;
	}
}
