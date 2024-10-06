using System;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Platform;

internal static partial class WebEvents
{
	private const string ImportString = "JSImports";

	internal static Action<Vector2Int>? ClientResized;

	[JSImport("WebEvents.CheckClientResized", ImportString)]
	internal static partial void CheckClientResized();

	[JSExport]
	internal static void InvokeClientResized(int width, int height)
		=> ClientResized?.Invoke(new Vector2Int(width, height));

	internal static Action? AnimationFrameRequested;

	[JSImport("WebEvents.RequestAnimationFrame", ImportString)]
	internal static partial void RequestAnimationFrame();

	[JSExport]
	internal static void InvokeAnimationFrameRequested()
		=> AnimationFrameRequested?.Invoke();

	internal static Action? WindowClosing;

	[JSExport]
	internal static void InvokeWindowClosing()
		=> WindowClosing?.Invoke();
}
