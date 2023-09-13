using Timer = System.Timers.Timer;

namespace SampleGame.Elements;

public class MemoryLogic
{
	private GameState _state = GameState.WaitingForFirstClick;

	private readonly Timer _timer;

	private MemoryField _field;
	private MemoryTile[] _revealedTiles = new MemoryTile[2];

	private int _remainingTiles;

	public MemoryLogic(MemoryField field)
	{
		_field = field;
		_field.OnTileClicked += processTileClicked;

		_timer = new Timer(500)
		{
			Enabled = false
		};
		_timer.Elapsed += timeoutElapsed;

		StartGame();
	}

	private void processTileClicked(int index)
	{
		if (_state == GameState.TimingOut) return;

		if (_state == GameState.Finished)
		{
			StartGame();

			_state = GameState.WaitingForFirstClick;
			return;
		}

		if (_field.Tiles[index].IsShown) return;

		_field.Tiles[index].Show();

		if (_state == GameState.WaitingForFirstClick)
		{
			_revealedTiles[0] = _field.Tiles[index];

			_state = GameState.WaitingForSecondClick;
			return;
		}

		if (_state == GameState.WaitingForSecondClick)
		{
			_revealedTiles[1] = _field.Tiles[index];
			if (_revealedTiles[0].TextureName != _revealedTiles[1].TextureName)
			{
				_timer.Start();
				_state = GameState.TimingOut;
				return;
			}

			_remainingTiles -= 2;

			if (_remainingTiles > 0)
			{
				_state = GameState.WaitingForFirstClick;
				return;
			}

			_state = GameState.Finished;
			return;
		}
	}

	private void timeoutElapsed(object? sender, System.Timers.ElapsedEventArgs e)
	{
		_timer.Stop();

		_revealedTiles[0].Hide();
		_revealedTiles[1].Hide();

		_state = GameState.WaitingForFirstClick;
	}

	public void Solve()
	{
		foreach (var tile in _field.Tiles)
		{
			tile.Show();
			_state = GameState.Finished;
		}
	}

	public void StartGame()
	{
		_field.GenerateTiles();
		_remainingTiles = _field.Tiles.Count;
	}

	public enum GameState
	{
		WaitingForFirstClick,
		WaitingForSecondClick,
		TimingOut,
		Finished
	}
}
