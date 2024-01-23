namespace Azalea.Platform.Windows;
internal readonly struct RawMouse
{
	public readonly ushort Flags;
	public readonly uint Buttons;
	public readonly uint RawButtons;
	public readonly int LastX;
	public readonly int LastY;
	public readonly uint ExtraInformation;
}
