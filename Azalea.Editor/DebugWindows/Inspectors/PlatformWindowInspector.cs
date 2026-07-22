using Azalea.Editor.Design.Gui;
using Azalea.Platform.Windowing;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformWindowInspector
{
	public static GUIWindow Create(PlatformWindow window, GUIWindow? origin = null)
	{
		var position = origin is null ? new(400, 400) : origin.Position + new Vector2(20, 20);
		var guiWindow = GUIWindow.Create("PlatformWindow", position, new(400, 400));

		guiWindow.AddLabel("Platform: " + window.PlatformType);
		guiWindow.AddLabel("Title: " + window.Title);
		guiWindow.AddLabel("HasRenderer: " + (window.SubscribedRenderer is not null).ToString());
		guiWindow.AddLabel("HasScheduler: " + (window.SubscribedScheduler is not null).ToString());
		guiWindow.AddLabel("Closed: " + window.Closed);
		guiWindow.AddButton("Close", window.Close);

		window.Closed.OnValueChanged += _ => guiWindow.Hide();
		return guiWindow;
	}
}
