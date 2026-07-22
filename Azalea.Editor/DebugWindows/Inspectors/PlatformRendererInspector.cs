using Azalea.Editor.Design.Gui;
using Azalea.Platform.Rendering;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformRendererInspector
{
	public static GUIWindow Create(PlatformRenderer renderer, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create("PlatformRenderer", position, new(400, 400));

		window.AddLabel("Stopped: " + renderer.Stopped);

		renderer.Stopped.OnValueChanged += _ => window.Hide();
		return window;
	}
}
