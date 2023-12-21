using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Platform.Glfw;
using System;

namespace Azalea.Platform.Desktop;
internal class DesktopGameHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private GLFWWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private GLRenderer? _renderer;

	internal override IInputManager InputManager => _input ?? throw new Exception("Cannot use InputManager before it is initialized");
	private GLFWInput? _input;

	public DesktopGameHost(HostPreferences preferences)
	{
		_window = new GLFWWindow(
			preferences.WindowTitle,
			preferences.PreferredClientSize.X, preferences.PreferredClientSize.Y,
			preferences.PreferredWindowState,
			preferences.WindowVisible,
			preferences.WindowResizable,
			preferences.DecoratedWindow,
			preferences.TransparentFramebuffer);
		_window.VSync = preferences.VSync;
	}

	public override void CallInitialized()
	{
		GL.Import();
		GL.Enable(GLCapability.Blend);
		GL.BlendFunc(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		_renderer = new GLRenderer(_window);
		_input = new GLFWInput(_window.Handle);

		base.CallInitialized();
	}
}
