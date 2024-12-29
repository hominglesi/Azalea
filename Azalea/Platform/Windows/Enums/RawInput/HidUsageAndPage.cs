using System;

namespace Azalea.Platform.Windows.Enums.RawInput;
internal readonly struct HidUsageAndPage : IEquatable<HidUsageAndPage>
{
	public readonly ushort Usage;
	public readonly ushort UsagePage;

	public bool Equals(HidUsageAndPage other)
		=> Usage == other.Usage && UsagePage == other.UsagePage;

	public override bool Equals(object? obj)
		=> obj is HidUsageAndPage usage && Equals(usage);

	public static bool operator ==(HidUsageAndPage usage1, HidUsageAndPage usage2)
		=> usage1.Equals(usage2);

	public static bool operator !=(HidUsageAndPage usage1, HidUsageAndPage usage2)
		=> usage1.Equals(usage2) == false;
}
