using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.IO.Resources;

namespace Azalea;

public abstract class AzaleaGame : Composition
{
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
