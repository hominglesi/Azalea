using Azalea;
using SampleGame;

new HostBuilder()
	.SetGameSize(new Vector2Int(876, 660))
	.Create()
	.Run(new MemoryGame());
