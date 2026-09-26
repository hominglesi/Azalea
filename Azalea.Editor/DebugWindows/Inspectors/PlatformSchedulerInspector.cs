using Azalea.Editor.Design.Gui;
using Azalea.Platform;
using Azalea.Platform.Scheduling;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformSchedulerInspector
{
	public static GUIWindow Create(Application app, PlatformScheduler scheduler, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create(app, "PlatformScheduler", position, new(400, 400));

		window.AddButton("Stop", scheduler.Stop);

		var thread = scheduler.Thread;
		window.AddGroup("Schedule Thread");
		window.AddButton("Inspect PlatformWindow",
			() => PlatformWindowInspector.Create(app, thread.Window, window));
		window.AddButton("Inspect PlatformRenderer",
			() => PlatformRendererInspector.Create(app, thread.Renderer, window));
		window.AddLabel("Has Protocol: " + (thread.Protocol is not null).ToString());
		GameThreadInspector.Inject(window, thread);
		window.FinishGroup();

		return window;
	}
}
