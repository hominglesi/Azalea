namespace Azalea.Platform.Windows;

//Mappings from https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongw
internal enum WindowLongValue
{
	/// <summary>
	/// Retrieves the
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles">extended window styles</see>.
	/// </summary>
	ExStyle = -20,
	/// <summary> Retrieves a handle to the application instance. </summary>
	HInstance = -6,
	/// <summary> Retrieves a handle to the parent window, if any. </summary>
	HWndParent = -8,
	/// <summary> Retrieves the identifier of the window. </summary>
	Id = -12,
	/// <summary> 
	/// Retrieves the <see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/window-styles">window styles</see>.
	/// </summary>
	Style = -16,
	/// <summary>
	/// Retrieves the user data associated with the window.
	/// This data is intended for use by the application that created the window. Its value is initially zero.
	/// </summary>
	UserData = -21,
	/// <summary>
	/// Retrieves the address of the window procedure, or a handle representing the address of the window procedure.
	/// You must use the <see cref="WinAPI.CallWindowProc"/> function to call the window procedure.
	/// </summary>
	WndProc = -4
}
