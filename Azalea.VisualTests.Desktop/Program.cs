using Azalea;
using Azalea.IO.Resources;
using Azalea.VisualTests;

//PerformanceTrace.Enabled = true;

Assets.SetupReflectedStore("../../../../../../Azalea.VisualTests/");

new HostBuilder()
	.SetTitle("Azalea Visual Tests")
	.SetGameSize(new Vector2Int(1600, 900))
	.SetResizable(true)
	.SetupPersistentDirectory("Azalea.VisualTests")
	.SetupConfig()
	.Create()
	.Run(new VisualTests());
