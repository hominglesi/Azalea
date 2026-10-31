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

		var app = host.CreateApplication(EditorWrapper.Wrap(new VisualTests()), "Azalea Visual Tests", new Vector2Int(1600, 900));

		host.WaitUntilClosed();
		host.SaveConfig();
	}
}

