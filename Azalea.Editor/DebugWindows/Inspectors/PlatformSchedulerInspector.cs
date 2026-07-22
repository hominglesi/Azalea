using Azalea.Editor.Design.Gui;
using Azalea.Platform.Scheduling;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformSchedulerInspector
{
	public static GUIWindow Create(PlatformScheduler scheduler, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create("PlatformScheduler", position, new(400, 400));

		window.AddButton("Stop", scheduler.Stop);

		var thread = scheduler.Thread;
		window.AddGroup("Schedule Thread");
		window.AddButton("Inspect PlatformWindow",
			() => PlatformWindowInspector.Create(thread.Window, window));
		window.AddButton("Inspect PlatformRenderer",
			() => PlatformRendererInspector.Create(thread.Renderer, window));
		window.AddLabel("Has Protocol: " + (thread.Protocol is not null).ToString());
		GameThreadInspector.Inject(window, thread);
		window.FinishGroup();

		return window;
	}
}
