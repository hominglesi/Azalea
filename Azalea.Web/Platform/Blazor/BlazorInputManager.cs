using Azalea.Inputs;
using System;

namespace Azalea.Web.Platform.Blazor;

public class BlazorInputManager
{
	public BlazorInputManager()
	{
		Initialize();
	}

	private void Initialize()
	{
		for (int i = 0; i < Input.MOUSE_BUTTONS.Length; i++)
		{
			Input.MOUSE_BUTTONS[i] = new ButtonState();
		}
		foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
		{
			if (Input.KEYBOARD_KEYS.ContainsKey((int)key)) continue;

			Input.KEYBOARD_KEYS.Add((int)key, new ButtonState());
		}
	}
}
