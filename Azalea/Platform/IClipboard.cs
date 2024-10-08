namespace Azalea.Platform;

public interface IClipboard
{
	/// <summary>
	/// Get text for clipboard
	/// </summary>
	public string? GetText();

	/// <summary>
	/// Copy text to clipboard
	/// </summary>
	public bool SetText(string text);
}
