using System;

namespace Azalea.Platform.Windows;

[Flags]
internal enum WindowStylesEx : uint
{
	/// <summary> The window accepts drag-drop files. </summary>
	AcceptFiles = 0x00000010,
	/// <summary> Forces a top-level window onto the taskbar when the window is visible. </summary>
	AppWindow = 0x00040000,
	ContextHelp = 0x00000400,
}
