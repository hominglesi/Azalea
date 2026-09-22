using Azalea.Design.Containers;
using Azalea.Editing;
using Azalea.Graphics;
using Azalea.Graphics.Camera;
using Azalea.Inputs.Events;
using Azalea.Inputs.Gamepads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Inputs;

public static class Input
{
	#region Gamepad

	private static IGamepadManager? _gamepadManager;

	internal static void SetGamepadManager(IGamepadManager gamepadManager)
		=> _gamepadManager = gamepadManager;

	public static IGamepad? GetGamepad(int index)
		=> _gamepadManager!.GetGamepad(index);

	#endregion
}
