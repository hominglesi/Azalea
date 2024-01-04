using Azalea.Graphics;
using System;

namespace Azalea.Debugging;
public static class Editor
{
	public static DebuggingOverlay Overlay => _overlay ?? throw new Exception("Debug overlay was not initialized");
	internal static DebuggingOverlay? _overlay;

	public static void InspectObject(GameObject obj)
	{
		Overlay.Inspector.SetObservedObject(obj);
	}

	public static void HighlightObject(GameObject obj)
	{
		Overlay.AddInternal(new DebugRectHighlight()
		{
			Depth = -5000,
			Position = obj.ScreenSpaceDrawQuad.TopLeft,
			Size = obj.ScreenSpaceDrawQuad.BottomRight - obj.ScreenSpaceDrawQuad.TopLeft,
		});
	}
}
