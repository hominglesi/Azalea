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

	internal override IInputManager InputManager => _input ?? throw new Exception("Cannot use InputManager before it is initialized");
	private GLFWInput? _input;

	public DesktopGameHost(HostPreferences preferences)
	{
		_window = new GLFWWindow(preferences.PreferredClientSize);
	}

	public override void CallInitialized()
	{
		GL.Import();
		GL.Enable(GLCapability.Blend);
		GL.BlendFunc(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);
		Console.WriteLine(GL.GetString(GLStringName.Version));

		_renderer = new GLRenderer(_window);
		_input = new GLFWInput(_window.Handle);

		base.CallInitialized();
	}
}
