using System;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web;

internal static partial class WebEvents
{
	internal static Vector2Int WindowSize;
	internal static Action<Vector2Int>? OnWindowResized;

	[JSExport]
	internal static void InvokeWindowResized(int width, int height)
	{
		WindowSize = new Vector2Int(width, height);
		OnWindowResized?.Invoke(WindowSize);
	}

	internal static Action? OnAnimationFrameRequested;

	[JSImport("WebEvents.RequestAnimationFrame", "JSImports")]
	internal static partial void RequestAnimationFrame();

	[JSExport]
	internal static void InvokeAnimationFrameRequested()
		=> OnAnimationFrameRequested?.Invoke();
}
