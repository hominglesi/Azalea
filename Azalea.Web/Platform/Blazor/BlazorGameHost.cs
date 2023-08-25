using Azalea.Graphics.Rendering;
using Azalea.Platform;
using Azalea.Web.Graphics.Blazor;
using System;

namespace Azalea.Web.Platform.Blazor;

public class BlazorGameHost : GameHost
{
	public override IWindow Window => _window;
	private readonly BlazorWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private BlazorRenderer? _renderer;

	private BlazorInputManager? _input;

	public BlazorGameHost()
	{
		_window = new BlazorWindow();
		_window.OnInitialized += CallInitialized;
		_window.OnUpdate += CallOnUpdate;
		_window.OnRender += CallOnRender;
	}

	public override void CallInitialized()
	{
		_renderer = new BlazorRenderer(_window.GL, _window);
		_input = new BlazorInputManager();

		base.CallInitialized();
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);
		_window.Run();
	}
}