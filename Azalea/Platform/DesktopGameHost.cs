﻿using Azalea.Editing;
using Azalea.Graphics.OpenGL;
using Azalea.Graphics.Rendering;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Platform.Windows;
using Azalea.Sounds;
using Azalea.Sounds.OpenAL;

namespace Azalea.Platform;
internal class DesktopGameHost : GameHost
{
	private readonly Vector2Int _defaultWindowSize = new(1280, 720);

	internal DesktopGameHost(HostPreferences prefs)
		: base(prefs)
	{
		if (prefs.PersistentDirectory is not null)
			Assets.SetupPersistentStore(prefs.PersistentDirectory);

		if (prefs.ReflectedDirectory is not null)
			Assets.SetupReflectedStore(prefs.ReflectedDirectory);

		if (prefs.ConfigName is not null)
			ConfigProvider = new FileConfigProvider(prefs.ConfigName);

		if (prefs.TracingEnabled)
			PerformanceTrace.Enabled = true;
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);

		ConfigProvider?.Save();
		((ALAudioManager)AudioManager)?.Dispose();
	}

	protected override void RunGameLoop()
	{
		var desktopWindow = (PlatformWindow)Window;
		while (desktopWindow.ShouldClose == false)
		{
			ProcessGameLoop();
		}

		Window.Hide();

		if (PerformanceTrace.Enabled)
		{
			using var traceStream = Assets.PersistentStore.GetOrCreateStream("trace.txt");
			PerformanceTrace.SaveEventsTo(traceStream);
		}

		Window.Dispose();
	}

	internal override IWindow CreateWindow(HostPreferences prefs)
	{
		var windowSize = prefs.GameSize ?? _defaultWindowSize;
		var resizable = prefs.Resizable ?? false;
		var startingState = prefs.StartingState ?? WindowState.Normal;
		var title = prefs.Title ?? "Azalea Game";
		var vSync = prefs.VSync ?? true;

		return new Win32Window(title, windowSize, startingState, false)
		{
			VSync = vSync,
			Resizable = resizable
		};
	}

	internal override IRenderer CreateRenderer(IWindow window)
		=> new GLRenderer(window);
	internal override IAudioManager CreateAudioManager()
		=> new ALAudioManager();
	internal override IClipboard CreateClipboard()
		=> new WindowsClipboard();
}
