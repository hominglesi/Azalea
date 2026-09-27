using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Platform.Windowing;

namespace Azalea;

public abstract class AzaleaGame : Composition
{
	public static AzaleaGame? RENDERED_GAME;

	public AzaleaGame()
	{
		RelativeSizeAxes = Axes.Both;

		Assets.MainStore.AddMsdfFont("Roboto-Regular", "Fonts/Roboto-Regular.csv", "Fonts/Roboto-Regular.bmp");
	}

	protected override void Initialize()
	{
		App.Window.SetIcon(Assets.MainStore.GetImage("Textures/azalea-icon.png"));
		base.Initialize();
	}
}
