using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Platform;
using Azalea.Sounds;
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

	public WebHost()
	{
		_window = new WebWindow();
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
		WebEvents.OnAnimationFrameRequested += runGameLoop;
		WebEvents.RequestAnimationFrame();
	}

	private void runGameLoop()
	{
		WebEvents.CheckClientSize();

		ProcessGameLoop();

		WebEvents.RequestAnimationFrame();
	}

	public override DateTime GetCurrentTime()
		=> WebEvents.GetCurrentPreciseTime();
}
