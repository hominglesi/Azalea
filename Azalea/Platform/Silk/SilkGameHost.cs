using Azalea.Graphics.OpenGL;
using Azalea.Graphics.Rendering;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Diagnostics;

namespace Azalea.Platform.Silk;

internal class SilkGameHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private SilkWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private GLRenderer? _renderer;

	private GL? _gl;

	private SilkInputManager? _inputManager;

	public SilkGameHost(HostPreferences preferences)
	{
		_window = new SilkWindow(preferences.PreferredClientSize, preferences.PreferredWindowState);

		_window.Window.Load += CallInitialized;
		_window.Window.Render += (_) => CallOnRender();
		_window.Window.Update += (deltaTime) =>
		{
			Time._deltaTime = (float)deltaTime;
			CallOnUpdate();
		};
	}

	public override void CallInitialized()
	{
		_gl = _window.Window.CreateOpenGL();
		_renderer = new GLRenderer(_gl, _window);
		_inputManager = new SilkInputManager(_window.Window.CreateInput());

		_window.InitializeAfterStartup(_gl, _inputManager);
		((SilkClipboard)Clipboard).SetInput(_inputManager);

		base.CallInitialized();
	}

	public override void CallOnUpdate()
	{
		base.CallOnUpdate();

		Debug.Assert(_gl is not null);

		_gl.Viewport(0, 0, (uint)_window.ClientSize.X, (uint)_window.ClientSize.Y);

		_inputManager?.Update();
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);
		_window.Window.Run();
	}

	protected override IClipboard? CreateClipboard() => new SilkClipboard();
}
