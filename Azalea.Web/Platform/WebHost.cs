using Azalea.Debugging;
using Azalea.Design.Containers;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Physics;
using Azalea.Platform;
using Azalea.Web.Rendering;
using System;

namespace Azalea.Web.Platform;

public class WebHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private WebWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private WebGLRenderer? _renderer;

	public WebHost()
	{
		_window = new WebWindow();
	}

	public override void CallInitialized()
	{
		WebGL.Enable(GLCapability.Blend);
		WebGL.BlendFuncSeparate(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha, GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		_renderer = new WebGLRenderer(_window);

		base.CallInitialized();
	}

	public override void Run(AzaleaGame game)
	{
		if (AzaleaSettings.EnableDebugging)
		{
			var root = new DebuggingOverlay();
			_root = root;
			Editor._overlay = root;
		}
		else
			_root = new Composition();

		_root.Add(game);

		game.SetHost(this);

		_physics = new PhysicsGenerator();
		_physics.UsesGravity = false;

		CallInitialized();

		lastFrameTime = WebEvents.GetCurrentPreciseTime();

		WebEvents.OnAnimationFrameRequested += runGameLoop;
		WebEvents.RequestAnimationFrame();
	}

	private DateTime lastFrameTime;
	private DateTime frameTime;
	private float deltaTime;

	private void runGameLoop()
	{
		WebEvents.CheckClientSize();

		frameTime = WebEvents.GetCurrentPreciseTime();
		deltaTime = (float)frameTime.Subtract(lastFrameTime).TotalSeconds;
		lastFrameTime = frameTime;

		Time.Update(deltaTime);

		WebEvents.HandleEvents();

		CallOnFixedUpdate();
		CallOnUpdate();
		CallOnRender();

		WebEvents.RequestAnimationFrame();
	}
}
