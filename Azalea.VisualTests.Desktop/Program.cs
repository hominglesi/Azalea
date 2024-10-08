using Azalea;
using Azalea.VisualTests;

//PerformanceTrace.Enabled = true;

new HostBuilder()
	.SetTitle("Azalea Visual Tests")
	.SetGameSize(new Vector2Int(1600, 900))
	.SetResizable(true)
	.SetupPersistentDirectory("Azalea.VisualTests")
	.SetupReflectedDirectory("../../../../../../Azalea.VisualTests/")
	.SetupConfig()
	.Create()
	.Run(new VisualTests());
