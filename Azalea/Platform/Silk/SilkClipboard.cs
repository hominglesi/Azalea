namespace Azalea.Platform.Silk;

public class SilkClipboard : IClipboard
{
	private SilkInputManager? _input;

	internal void SetInput(SilkInputManager input) => _input = input;

	public string? GetText()
	{
		if (_input?.PrimaryKeyboard is null) return null;
		return _input?.PrimaryKeyboard.ClipboardText;
	}

	public void SetText(string text)
	{
		if (_input?.PrimaryKeyboard is null) return;
		_input.PrimaryKeyboard.ClipboardText = text;
	}
}
