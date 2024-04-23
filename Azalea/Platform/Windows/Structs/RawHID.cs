using System;

namespace Azalea.Platform.Windows;
internal readonly struct RawHID
{
	public readonly uint SizeHid;
	public readonly uint Count;
	public readonly IntPtr RawData;
}
