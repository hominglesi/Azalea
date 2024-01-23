using System;

namespace Azalea.Platform.Windows;

[Flags]
//Mappings from https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfrompoint
internal enum MonitorFromFlags : uint
{
	/// <summary> Returns NULL. </summary>
	DefaultToNull = 0x00000000,
	/// <summary> Returns a handle to the primary display monitor. </summary>
	DefaultToPrimary = 0x00000001,
	/// <summary> Returns a handle to the display monitor that is nearest to the point. </summary>
	DefaultToNearest = 0x00000002,
}
