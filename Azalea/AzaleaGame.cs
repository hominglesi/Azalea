using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
using System;

namespace Azalea;

public abstract class AzaleaGame : Composition
{
	public static AzaleaGame Main => _main ?? throw new Exception("No game has been initialized");
	private static AzaleaGame? _main;

	public SceneContainer SceneManager => _sceneManager ?? throw new Exception("Game has not been initialized");
	private SceneContainer? _sceneManager;

	internal virtual void SetHost(GameHost host)
	{
		_main ??= this;

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

		AddInternal(_sceneManager = new SceneContainer());

		OnInitialize();
	}

	protected virtual void OnInitialize() { }
}
