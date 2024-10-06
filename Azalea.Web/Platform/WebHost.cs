using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.IO.Configs;
using Azalea.Platform;
using Azalea.Sounds;
using Azalea.Web.IO;
using Azalea.Web.Rendering;
using Azalea.Web.Sounds;
using System;

namespace Azalea.Web.Platform;

public class WebHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private WebWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private WebGLRenderer? _renderer;

	public override IAudioManager AudioManager => _audioManager ?? throw new Exception("Cannot use AudioManager before it is initialized");
	private WebAudioManager? _audioManager;

	public override IConfigProvider ConfigProvider => _configProvider;
	private WebConfigProvider _configProvider;

	internal WebHost(HostBuilder.HostPreferences prefs)
	{
		CheckForbidden(prefs.GameSize, "Cannot set game size when running on the web.");
		CheckForbidden(prefs.Resizable, "Cannot set resizable property when running on the web.");
		CheckForbidden(prefs.StartingState, "Cannot set default state when running on the web.");
		CheckForbidden(prefs.VSync, "Cannot set vsync property when running on the web.");
		CheckForbidden(prefs.PersistentDirectory, "Cannot setup persistent directory when running on the web.");
		CheckForbidden(prefs.ConfigName, "Config is automatically set up when running on the web.");

		var title = prefs.Title ?? "AzaleaGame";

		_window = new WebWindow
		{
			Title = title
		};

		_configProvider = new WebConfigProvider();

		_window.Closing += beforeClose;
	}

	public override void CallInitialized()
	{
		WebGL.Enable(GLCapability.Blend);
		WebGL.BlendFuncSeparate(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha, GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		_renderer = new WebGLRenderer(_window);
		_audioManager = new WebAudioManager();

		base.CallInitialized();
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
		_configProvider.Save();
	}

	public override DateTime GetCurrentTime()
		=> WebFunctions.GetCurrentPreciseTime();
}
