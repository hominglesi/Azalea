namespace Azalea.Platform.Veldrid;
/*
public class VeldridWindow : IWindow
{
	public GraphicsDevice GraphicsDevice { get; private set; }
	public Sdl2Window Window;

	public Action? OnInitialized;
	public Action? OnInput;
	public Action? OnUpdate;
	public Action? OnRender;

	private Stopwatch _stopwatch;

	public VeldridWindow(Vector2Int preferredClientSize, WindowState preferredWindowState)
	{
		var position = IWindow.AproximateCenterWindowPosition(preferredClientSize);
		var windowCreateInfo = new WindowCreateInfo()
		{
			X = position.X,
			Y = position.Y,
			WindowWidth = preferredClientSize.X,
			WindowHeight = preferredClientSize.Y,
			WindowTitle = IWindow.DefaultTitle,
			WindowInitialState = preferredWindowState.ToVeldridWindowState()
		};
		Window = VeldridStartup.CreateWindow(windowCreateInfo);

		GraphicsDeviceOptions options = new()
		{
			PreferStandardClipSpaceYDirection = true,
			PreferDepthRangeZeroToOne = true
		};
		GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options);

		Window.Resizable = false;
		Window.Resized += onResized;

		_stopwatch = new Stopwatch();
	}

	private void onResized()
	{
		GraphicsDevice.ResizeMainWindow((uint)ClientSize.X, (uint)ClientSize.Y);
	}

	public void Run()
	{
		OnInitialized?.Invoke();
		while (Window.Exists)
		{
			//Get all inputs first
			OnInput?.Invoke();

			//Invoke all updates and calculate delta time
			if (_stopwatch.IsRunning == false) _stopwatch.Start();

			Time._deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
			_stopwatch.Restart();

			OnUpdate?.Invoke();

			//Render to screen
			OnRender?.Invoke();
		}
	}

	public Vector2Int ClientSize
	{
		get => new(Window.Width, Window.Height);
		set { Window.Width = value.X; Window.Height = value.Y; }
	}

	public Vector2Int Position
	{
		get => new(Window.X, Window.Y);
		set { Window.X = value.X; Window.Y = value.Y; }
	}

	public string Title { get => Window.Title; set => Window.Title = value; }

	public WindowState State
	{
		get => Window.WindowState.ToAzaleaWindowState();
		set => Window.WindowState = value.ToVeldridWindowState();
	}

	public bool Resizable
	{
		get => Window.Resizable;
		set => Window.Resizable = value;
	}

	public bool CursorVisible
	{
		get => Window.CursorVisible;
		set => Window.CursorVisible = value;
	}

	public unsafe void Center()
	{
		SDL_DisplayMode displayMode;
		var result = Sdl2Native.SDL_GetCurrentDisplayMode(0, &displayMode);
		if (result != 0) return;

		var monitorSize = new Vector2Int(displayMode.w, displayMode.h);
		Position = new Vector2Int(
			monitorSize.X / 2 - ClientSize.X / 2,
			monitorSize.Y / 2 - ClientSize.Y / 2
			) + IWindow.CenterOffset;
	}

	public unsafe void SetIconFromStream(Stream imageStream)
	{
		using var image = Image.Load<Rgba32>(imageStream);
		var pixels = image.CreateReadOnlyPixelSpan();

		var pitch = image.Width * sizeof(Rgba32);

		fixed (void* pixelSpan = pixels.Span)
		{
			var surface = VeldridExtentions.SDL_CreateRGBSurfaceFrom(pixelSpan, image.Width, image.Height, 32, pitch, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
			VeldridExtentions.SDL_SetWindowIcon(Window.SdlWindowHandle, surface);

			VeldridExtentions.SDL_FreeSurface(surface);
		}
	}

	public void Close()
	{
		Window.Close();
	}
}*/
