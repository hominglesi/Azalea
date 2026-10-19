using Azalea;
using Azalea.Editor;
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

		host.CreateApplication(EditorWrapper.Wrap(new VisualTests()), "Azalea Visual Tests", new Vector2Int(1600, 900));

		host.WaitUntilEmpty();
		host.SaveConfig();
	}
}

