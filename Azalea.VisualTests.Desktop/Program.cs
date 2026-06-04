using Azalea;
using Azalea.Editor;
using Azalea.VisualTests;
using System;

internal class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		var host = new HostBuilder()
			.EnableEditor()
			.SetTitle("Azalea Visual Tests")
			.SetGameSize(new Vector2Int(1600, 900))
			.SetResizable(true)
			.SetVSync(true)
			.SetupPersistentDirectory("Azalea.VisualTests")
			.SetupReflectedDirectory("../../../../../../Azalea.VisualTests/")
			//.EnableTracing()
			.SetupConfig()
			.Create();

		AzaleaGame.RENDERED_GAME = EditorWrapper.Wrap(new VisualTests());
		host.Run(AzaleaGame.RENDERED_GAME);
	}
}

