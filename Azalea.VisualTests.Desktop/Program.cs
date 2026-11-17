using Azalea;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Scenes;
using Azalea.Scenes.Drawables2D;
using Azalea.VisualTests;
using System;

internal class Program
{
	[STAThread]
	private static void Main(string[] _)
	{
		var host = GameHost.Create();
		host.SetupPersistentDirectory("Azalea.VisualTests");
		host.SetupReflectedDirectory("../../../../../../Azalea.VisualTests/");
		host.SetupConfig();

		Assets.AddToMainStore(new NamespacedResourceStore(new EmbeddedResourceStore(typeof(VisualTests).Assembly), "Resources"));
		Assets.MainStore.AddMsdfFont("TitanOne-Regular", "Fonts/TitanOne-Regular.csv", "Fonts/TitanOne-Regular.bmp");

		var app = host.StartApplication(new Application(), "Azalea Visual Tests", new Vector2Int(1600, 900));
		//app.Scene = EditorWrapper.Wrap(new VisualTests());
		app.Scene = new Scene2D()
		{
			Child = new Container()
			{
				RelativeSizeAxes = Axes.Both,
				ChildAspectRatio = 16f / 9,
				ChildSpace = new(1600, 900),
				Children = [
					new Sprite(){
						RelativeSizeAxes = Axes.Both,
						Texture = Assets.WhitePixel
					},
					new Sprite(){
						Position = new(20),
						Size = new(150),
						Texture = Assets.GetTexture("Textures/azalea-icon.png")
					},
					new Sprite(){
						Position = new(50, 300),
						Size = new(75),
						Child = new Sprite()
						{
							RelativeSizeAxes = Axes.Both,
							Texture = Assets.GetTexture("Textures/azalea-icon.png")
						}
					}
				]
			}
		};

		host.WaitUntilClosed();
		host.SaveConfig();
	}
}

