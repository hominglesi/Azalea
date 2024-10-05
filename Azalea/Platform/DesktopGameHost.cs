using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Platform.Windows;
using Azalea.Sounds;
using Azalea.Sounds.OpenAL;
using System;

namespace Azalea.Platform;
internal class DesktopGameHost : GameHost
{
	private readonly Vector2Int _defaultWindowSize = new(1280, 720);

	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private Win32Window _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private GLRenderer? _renderer;

	public override IAudioManager AudioManager => _audioManager ?? throw new Exception("Cannot use AudioManager before it is initialized");
	private ALAudioManager? _audioManager;

	public override IConfigProvider ConfigProvider => _configProvider ?? throw new Exception("Cannot use ConfigProvider before it is initialized");
	private FileConfigProvider? _configProvider;

	internal DesktopGameHost(HostBuilder.HostPreferences prefs)
	{
		var windowSize = prefs.GameSize ?? _defaultWindowSize;
		var resizable = prefs.Resizable ?? false;
		var startingState = prefs.StartingState ?? WindowState.Normal;
		var title = prefs.Title ?? "Azalea Game";
		var vSync = prefs.VSync ?? true;

		_window = new Win32Window(title, windowSize, startingState, false)
		{
			VSync = vSync,
			Resizable = resizable
		};

		if (prefs.PersistentDirectory is not null)
			Assets.SetupPersistentStore(prefs.PersistentDirectory);

		if (prefs.ConfigName is not null)
			_configProvider = new FileConfigProvider(prefs.ConfigName);
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

		_configProvider?.Save();
		_audioManager?.Dispose();
	}
}
