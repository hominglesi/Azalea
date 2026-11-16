using Azalea;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Scenes;
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
			Children = [
				new Sprite(){
					Position = new(400),
					Size = new(150),
					Texture = Assets.GetTexture("Textures/azalea-icon.png")
				},
				new Sprite(){
					Position = new(800, 300),
					Size = new(75)
				}
			]
		};

		host.WaitUntilClosed();
		host.SaveConfig();
	}
}

