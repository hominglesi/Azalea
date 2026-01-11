using Azalea;
using Azalea.Editor;
using Azalea.VisualTests;
using System;

internal class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		new HostBuilder()
			.EnableEditor()
			.SetTitle("Azalea Visual Tests")
			.SetGameSize(new Vector2Int(1600, 900))
			.SetResizable(true)
			.SetVSync(true)
			.SetupPersistentDirectory("Azalea.VisualTests")
			.SetupReflectedDirectory("../../../../../../Azalea.VisualTests/")
			.SetupConfig()
			.Create()
		//.Run(new EditorWrapper(null));
		.Run(new EditorWrapper(new VisualTests()));
		//.Run(new VisualTests());
	}
}

