using Azalea;
using Azalea.Editor;
using Azalea.IO.Resources;
using Azalea.Platform;
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

		host.StartApplication(new Application());
		// Will start a functional but empty window

		host.StartApplication(new Application() { Scene = new VisualTests() });
		// OR
		var app = host.StartApplication(new Application());
		app.Scene = new VisualTests();
		// Manually setting the scene

		host.StartApplication(new MusicPlayer());
		class MusicPlayer : Application
	{
		public MusicPlayer()
		{
			Scene = new LibraryScene();
		}
	}
	// Letting the application dictate scenes

	var app = host.StartApplication(new Application(), "Azalea Visual Tests", new Vector2Int(1600, 900));
	app.Scene = EditorWrapper.Wrap(new VisualTests());

		var app = host.StartApplication(new Application(), "Azalea Visual Tests", new Vector2Int(1600, 900));
	app.Scene = EditorWrapper.Wrap(new VisualTests());


		host.WaitUntilClosed();
		host.SaveConfig();
	}
}

