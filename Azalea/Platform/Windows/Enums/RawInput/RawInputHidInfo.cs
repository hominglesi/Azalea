namespace Azalea.Platform.Windows.Enums.RawInput;
internal readonly struct RawInputHidInfo
{
	public readonly uint VendorId;
	public readonly uint ProductId;
	public readonly uint VersionNumber;
	public readonly ushort UsagePage;
	public readonly ushort Usage;
}
