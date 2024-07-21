using Azalea.Inputs;

namespace Azalea.Utils;
public static class InputUtils
{
	public static void SimulateCharInput(string charString)
	{
		foreach (var chr in charString)
			Input.ExecuteTextInput(chr);
	}

	public static void SimulateKeyInput(Keys key)
	{
		Input.ExecuteKeyboardKeyStateChange(key, true);
		Input.ExecuteKeyboardKeyStateChange(key, false);
	}

	public static void SimulateMultipleKeyInput(Keys key, int count)
	{
		for (int i = 0; i < count; i++)
			SimulateKeyInput(key);
	}
}
