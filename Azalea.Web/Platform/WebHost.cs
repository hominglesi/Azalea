using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.IO.Resources;
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
		_window = new WebWindow() { };
	}

	public override void CallInitialized()
	{
		WebGL.Enable(GLCapability.Blend);
		WebGL.BlendFuncSeparate(GLBlendFunction.SrcAlpha, GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		_renderer = new WebGLRenderer(_window);

		base.CallInitialized();
	}

	public override void Run(AzaleaGame game)
	{
		WebGL.ClearColor(0.5f, 0.5f, 1f, 1f);
		WebGL.Clear(GLBufferBit.Color);

		_indexBuffer = new WebGLIndexBuffer();
		_indexBuffer.SetData(_indices, GLUsageHint.StaticDraw);

		WebEvents.OnAnimationFrameRequested += runGameLoop;
		WebEvents.RequestAnimationFrame();
	}

	private WebGLIndexBuffer _indexBuffer;
	private int[] _indices = [0, 1, 2, 1, 3, 2];

	private void runGameLoop()
	{
		Console.WriteLine(Assets.GetText("Text/test.txt"));
		WebEvents.RequestAnimationFrame();
	}
}
