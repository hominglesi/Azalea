using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Platform.Windows;
using Azalea.Sounds;
using Azalea.Sounds.OpenAL;
using System;

namespace Azalea.Platform;
internal class DesktopGameHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private Win32Window _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private GLRenderer? _renderer;

	public override IAudioManager AudioManager => _audioManager ?? throw new Exception("Cannot use AudioManager before it is initialized");
	private ALAudioManager? _audioManager;

	public DesktopGameHost(HostPreferences prefs)
	{
		_window = new Win32Window(prefs.WindowTitle, prefs.ClientSize, prefs.WindowState, prefs.WindowVisible)
		{
			//These are fine just being set normaly
			VSync = prefs.VSync,
			Resizable = prefs.WindowResizable
		};
	}

	public override void CallInitialized()
	{
		GL.Enable(GLCapability.Blend);
		GL.BlendFunc(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		_renderer = new GLRenderer(_window);
		_audioManager = new ALAudioManager();

		base.CallInitialized();
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);

		_audioManager?.Dispose();
	}
}
