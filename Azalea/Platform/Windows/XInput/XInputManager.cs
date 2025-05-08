using Azalea.Inputs;
using Azalea.Inputs.Gamepads;
using Azalea.Platform.Windows.XInput;

namespace Azalea.Platform.Windows;
internal class XInputManager : IGamepadManager
{
	private const int _gamepadMaxCount = 4;

	private readonly int[] _packetNumbers = new int[_gamepadMaxCount];
	private readonly XInputGamepad[] _gamepads = new XInputGamepad[_gamepadMaxCount];

	public XInputManager()
	{
		for (int i = 0; i < _gamepadMaxCount; i++)
			_packetNumbers[i] = int.MinValue;

		for (int i = 0; i < _gamepadMaxCount; i++)
			_gamepads[i] = new XInputGamepad();
	}

	public void Update()
	{
		for (int i = 0; i < _gamepadMaxCount; i++)
		{
			XInputState gamepadState = new();
			if (WinAPI.XInputGetState(i, ref gamepadState) != 0)
			{
				_gamepads[i].IsConnected = false;
				continue;
			}

			//Skip gamepads that havent changed
			if (gamepadState.PacketNumber == _packetNumbers[i])
				continue;

			_gamepads[i].Update(gamepadState.Gamepad);
		}
	}

	public IGamepad? GetGamepad(int id)
		=> _gamepads[id].IsConnected ? _gamepads[id] : null;
}
