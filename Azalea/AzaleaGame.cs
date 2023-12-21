using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
using System;

namespace Azalea;

public abstract class AzaleaGame : Composition
{
	public static AzaleaGame Main => _main ?? throw new Exception("No game has been initialized");
	private static AzaleaGame? _main;

	public GameHost Host => _host ?? throw new Exception("GameHost has not been set");
	private GameHost? _host;

	internal virtual void SetHost(GameHost host)
	{
		_main ??= this;

		_host = host;

		Host.Initialized += CallInitialize;
	}

	public AzaleaGame()
	{
		RelativeSizeAxes = Axes.Both;
	}

	internal void CallInitialize()
	{
		Assets.AddFont(@"Fonts/Roboto-Regular.bin", "");
		Assets.AddFont(@"Fonts/Roboto-Regular.bin", "Roboto-Regular");

		Host.Window.SetIconFromStream(Assets.GetStream("Textures/azalea-icon.png")!);
		Host.Window.Center();

		OnInitialize();
	}

	protected virtual void OnInitialize() { }
}
