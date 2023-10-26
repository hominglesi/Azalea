using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using System;

namespace Azalea.Platform.Desktop;
public class DesktopGameHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private GLFWWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private GLRenderer? _renderer;

	public DesktopGameHost(HostPreferences preferences)
	{
		_window = new GLFWWindow(preferences.PreferredClientSize);

		_window.Initialized += CallInitialized;
		_window.Render += CallOnRender;
		_window.Update += CallOnUpdate;
	}

	public override void CallInitialized()
	{
		GL.Import();
		Console.WriteLine(GL.GetString(GLStringName.Version));
		_renderer = new GLRenderer(_window);

		base.CallInitialized();
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);
		_window.Run();
	}
}
