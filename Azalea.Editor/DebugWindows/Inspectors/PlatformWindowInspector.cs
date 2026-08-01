using Azalea.Editor.Design.Gui;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformWindowInspector
{
	public static GUIWindow Create(PlatformWindow window, GUIWindow? origin = null)
	{
		var position = origin is null ? new(400, 400) : origin.Position + new Vector2(20, 20);
		var guiWindow = GUIWindow.Create("PlatformWindow", position, new(400, 400));

		guiWindow.AddLabel("Platform: " + window.PlatformType);

		if (window is WindowsWindow winWindow)
		{
			guiWindow.AddGroup("Windows Window");
			guiWindow.AddLabel("Class Atom: " + winWindow.ClassAtom);
			guiWindow.AddLabel("Handle: " + winWindow.Handle);
			guiWindow.FinishGroup();
		}

		guiWindow.AddLabel("Title: " + window.Title);
		guiWindow.AddObservingLabel("Shown", window.Shown);

		guiWindow.AddObservingLabel("Position", window.Position);
		guiWindow.AddObservingLabel("Client Position", window.ClientPosition);
		guiWindow.AddObservingLabel("Client Size", window.ClientSize);

		// For now using a window without these doesn't make sense
		// so we'll just assume that they are created
		Debug.Assert(window.SubscribedRenderer is not null);
		Debug.Assert(window.SubscribedScheduler is not null);

		guiWindow.AddButton("Inspect Subscribed Renderer",
			() => PlatformRendererInspector.Create(window.SubscribedRenderer, guiWindow));
		guiWindow.AddButton("Inspect Subscribed Scheduler",
			() => PlatformSchedulerInspector.Create(window.SubscribedScheduler, guiWindow));

		guiWindow.AddLabel("Device Context Borrowed: " + window.DeviceContextBorrowed);

		guiWindow.AddObservingLabel("Closed", window.Closed);
		guiWindow.AddButton("Close", window.Close);

		var thread = window.Thread;
		guiWindow.AddGroup("Window Thread");
		GameThreadInspector.Inject(guiWindow, window.Thread);
		guiWindow.FinishGroup();

		return guiWindow;
	}
}
