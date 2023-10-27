namespace Azalea.Platform.Veldrid;
/*
public class VeldridGameHost : GameHost
{
	public override IWindow Window => _window;
	private readonly VeldridWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private VeldridRenderer? _renderer;

	private VeldridInputManager? _inputManager;

	public VeldridGameHost(HostPreferences preferences)
	{
		_window = new VeldridWindow(preferences.PreferredClientSize, preferences.PreferredWindowState);
		_window.OnInitialized += CallInitialized;
		_window.OnUpdate += CallOnUpdate;
		_window.OnRender += CallOnRender;
	}

	public override void CallInitialized()
	{
		_renderer = new VeldridRenderer(_window.GraphicsDevice, _window);

		_inputManager = new VeldridInputManager(_window);

		base.CallInitialized();
	}

	public override void CallOnUpdate()
	{
		base.CallOnUpdate();
	}

	public override void Run(AzaleaGame game)
	{
		_window.Run();
		base.Run(game);
	}

	protected override IClipboard? CreateClipboard() => new VeldridClipboard();
}
*/
