using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform;

namespace Azalea.Utils;
public static class InputUtils
{
	public static void SimulateCharInput(Application app, string charString)
	{
		foreach (var chr in charString)
			app.Window.EnqueueInputEvent(new CharInputEvent(chr));
	}

	public static void SimulateKeyInput(Application app, Keys key)
	{
		app.Window.EnqueueInputEvent(new KeyDownEvent(key, false));
		app.Window.EnqueueInputEvent(new KeyUpEvent(key));
	}

	public static void SimulateMultipleKeyInput(Keys key, int count)
	{
		for (int i = 0; i < count; i++)
			SimulateKeyInput(key);
	}
}
