using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;

namespace Azalea;

public abstract class AzaleaGame : Composition
{
	public AzaleaGame()
	{
		RelativeSizeAxes = Axes.Both;

		Assets.MainStore.AddMsdfFont("Roboto-Regular", "Fonts/Roboto-Regular.csv", "Fonts/Roboto-Regular.bmp");

		Window.SetIconFromStream(Assets.GetStream("Textures/azalea-icon.png")!);
		Window.Center();
	}
}
