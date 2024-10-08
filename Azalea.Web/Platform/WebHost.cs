using Azalea.Graphics.Rendering;
using Azalea.Platform;
using Azalea.Sounds;
using Azalea.Web.IO;
using Azalea.Web.Rendering;
using Azalea.Web.Sounds;
using System;

namespace Azalea.Web.Platform;

public class WebHost : GameHost
{
	internal WebHost(HostPreferences prefs)
		: base(prefs)
	{
		CheckForbidden(prefs.PersistentDirectory, "Cannot setup persistent directory when running on the web.");
		CheckForbidden(prefs.ReflectedDirectory, "Cannot setup reflected directory when running on the web.");
		CheckForbidden(prefs.ConfigName, "Config is automatically set up when running on the web.");

		ConfigProvider = new WebConfigProvider();

		Window.Closing += beforeClose;
	}

	protected override void RunGameLoop()
	{
		WebEvents.AnimationFrameRequested += runGameLoop;
		WebEvents.RequestAnimationFrame();
	}

	private void runGameLoop()
	{
		WebEvents.CheckClientResized();

		ProcessGameLoop();

		WebEvents.RequestAnimationFrame();
	}

	private void beforeClose()
	{
		ConfigProvider?.Save();
	}

	internal override IWindow CreateWindow(HostPreferences prefs)
	{
		CheckForbidden(prefs.GameSize, "Cannot set game size when running on the web.");
		CheckForbidden(prefs.Resizable, "Cannot set resizable property when running on the web.");
		CheckForbidden(prefs.StartingState, "Cannot set default state when running on the web.");
		CheckForbidden(prefs.VSync, "Cannot set vsync property when running on the web.");

		var title = prefs.Title ?? "AzaleaGame";

		return new WebWindow
		{
			Title = title
		};
	}

	internal override IRenderer CreateRenderer(IWindow window)
		=> new WebGLRenderer(window);
	internal override IAudioManager CreateAudioManager()
		=> new WebAudioManager();
	internal override IClipboard CreateClipboard()
		=> new WebClipboard();

	public override DateTime GetCurrentTime()
		=> WebFunctions.GetCurrentPreciseTime();
}
