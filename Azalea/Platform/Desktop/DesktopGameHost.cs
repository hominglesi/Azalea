using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Platform.Windows;
using System;

namespace Azalea.Platform.Desktop;
internal class DesktopGameHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private Win32Window _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private GLRenderer? _renderer;

	internal override IInputManager InputManager => _input ?? throw new Exception("Cannot use InputManager before it is initialized");
	private GLFWInput? _input;

	public DesktopGameHost(HostPreferences preferences)
	{
		_window = new Win32Window(preferences.WindowTitle, preferences.PreferredClientSize,
			preferences.PreferredWindowState, preferences.WindowVisible)
		{
			//These are fine just being set normaly
			VSync = preferences.VSync,
			Resizable = preferences.WindowResizable
		};
	}

	public override void CallInitialized()
	{
		GL.Enable(GLCapability.Blend);
		GL.BlendFunc(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		_renderer = new GLRenderer(_window);
		//_input = new GLFWInput(_window.Handle);

		base.CallInitialized();
	}
}
