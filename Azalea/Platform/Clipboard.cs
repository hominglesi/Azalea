namespace Azalea.Platform;
public static class Clipboard
{
	public static string? GetText() => GameHost.Main.Clipboard.GetText();
	public static bool SetText(string text) => GameHost.Main.Clipboard.SetText(text);
}
