namespace Azalea.Platform.Windows;

//Mappings from https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
internal enum ShowWindowCommand : int
{
	/// <summary>
	/// Hides the window and activates another window.
	/// </summary>
	Hide = 0,
	/// <summary>
	/// Activates and displays a window. If the window is minimized, maximized, or arranged,
	/// the system restores it to its original size and position.
	/// An application should specify this flag when displaying the window for the first time.
	/// </summary>
	ShowNormal = 1,
	/// <summary>
	/// Activates and displays a window. If the window is minimized, maximized, or arranged,
	/// the system restores it to its original size and position.
	/// An application should specify this flag when displaying the window for the first time.
	/// Same as <see cref="ShowNormal"/>
	/// </summary>
	Normal = ShowNormal,
	/// <summary>
	/// Activates the window and displays it as a minimized window.
	/// </summary>
	ShowMinimized = 2,
	/// <summary>
	/// Activates the window and displays it as a maximized window.
	/// </summary>
	ShowMaximized = 3,
	/// <summary>
	/// Activates the window and displays it as a maximized window. Same as <see cref="ShowMaximized"/>
	/// </summary>
	Maximize = ShowMaximized,
	/// <summary>
	/// Displays a window in its most recent size and position.
	/// This value is similar to <see cref="ShowNormal"/>, except that the window is not activated.
	/// </summary>
	ShowNoActivate = 4,
	/// <summary>
	/// Activates the window and displays it in its current size and position.
	/// </summary>
	Show = 5,
	/// <summary>
	/// Minimizes the specified window and activates the next top-level window in the Z order.
	/// </summary>
	Minimize = 6,
	/// <summary>
	/// Displays the window as a minimized window.
	/// This value is similar to <see cref="ShowMinimized"/>, except the window is not activated.
	/// </summary>
	ShowMinNoActive = 7,
	/// <summary>
	/// Displays the window in its current size and position.
	/// This value is similar to <see cref="Show"/>, except that the window is not activated.
	/// </summary>
	ShowNoActive = 8,
	/// <summary>
	/// Activates and displays the window. If the window is minimized, maximized, or arranged,
	/// the system restores it to its original size and position.
	/// An application should specify this flag when restoring a minimized window.
	/// </summary>
	Restore = 9,
}
