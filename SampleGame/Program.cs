using Azalea;
using SampleGame;

new HostBuilder()
	.EnableEditor()
	.SetGameSize(new Vector2Int(876, 660))
	.SetupReflectedDirectory("../../../../")
	.Create()
	.Run(new MemoryGame());
