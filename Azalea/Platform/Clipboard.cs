namespace Azalea.Platform;
public static class Clipboard
{
	private static IClipboard? _instance;
	public static IClipboard Instance => _instance ??= GameHost.Main.Clipboard;

	public static string? GetText() => Instance.GetText();
	public static bool SetText(string text) => Instance.SetText(text);
}
