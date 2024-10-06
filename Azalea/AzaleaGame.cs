using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
using System;

namespace Azalea;

public abstract class AzaleaGame : Composition
{
	public SceneContainer SceneManager => _sceneManager ?? throw new Exception("Game has not been initialized");
	private SceneContainer? _sceneManager;

	internal virtual void SetHost(GameHost host)
	{
		// Set initial host
	}

	public AzaleaGame()
	{
		RelativeSizeAxes = Axes.Both;

		Assets.AddFont(@"Fonts/Roboto-Regular.bin", "");
		Assets.AddFont(@"Fonts/Roboto-Regular.bin", "Roboto-Regular");

		Host.Window.SetIconFromStream(Assets.GetStream("Textures/azalea-icon.png")!);
		Host.Window.Center();

		AddInternal(_sceneManager = new SceneContainer());
	}
}
