using System;

namespace Azalea.Inputs;

public class TextInputSource
{
	public event Action<string>? OnTextInput;

	public void TriggerTextInput(string text)
	{
		OnTextInput?.Invoke(text);
	}
}
