using Azalea.Editor.Design.Gui;
using Azalea.IO.Resources;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using System;
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

		guiWindow.AddObservingLabel("Title", window.Title);
		guiWindow.AddButton("Set Random Title", () => window.SetTitle("Random Title"));
		guiWindow.AddObservingLabel("Shown", window.Shown);
		guiWindow.AddButton("Show", window.Show);
		guiWindow.AddButton("Hide", window.Hide);
		guiWindow.AddObservingLabel("Position", window.Position);
		guiWindow.AddButton("Set Position to {100, 100}", () => window.SetPosition(new(100)));
		guiWindow.AddObservingLabel("Client Position", window.ClientPosition);
		guiWindow.AddButton("Set Client Position to {100, 100}", () => window.SetClientPosition(new(100)));
		guiWindow.AddObservingLabel("Size", window.Size);
		guiWindow.AddButton("Set Size to {800, 600}", () => window.SetSize(new(800, 600)));
		guiWindow.AddObservingLabel("Client Size", window.ClientSize);
		guiWindow.AddButton("Set Client Size to {800, 600}", () => window.SetClientSize(new(800, 600)));
		guiWindow.AddObservingLabel("Cursor Visible", window.CursorVisible);
		guiWindow.AddButton("Toggle Cursor Visible", () => window.SetCursorVisible(!window.CursorVisible.Value));

		guiWindow.AddButton("Focus", window.Focus);
		guiWindow.AddButton("Restore", window.Restore);
		guiWindow.AddButton("Maximize", window.Maximize);
		guiWindow.AddButton("Minimize", window.Minimize);
		guiWindow.AddButton("Fullscreen", window.Fullscreen);
		guiWindow.AddButton("Center", window.Center);
		guiWindow.AddButton("Request Attention", window.RequestAttention);
		guiWindow.AddButton("Set Azalea icon", () => window.SetIcon(Assets.MainStore.GetImage("Textures/azalea-icon.png")));
		guiWindow.AddButton("Set null icon", () => window.SetIcon(null));
		guiWindow.AddButton("Create tray icon", () =>
		{
			var trayIcon = window.CreateTrayIcon("Azalea Window", Assets.MainStore.GetImage("Textures/azalea-icon.png")!);
			trayIcon.OnClick += button => Console.WriteLine($"Tray icon recieved {button} click!");
			trayIcon.OnDoubleClick += () => Console.WriteLine($"Tray icon revieved double click!");
		});

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
