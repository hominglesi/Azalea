namespace Azalea.Platform.Windows;

//Mappings from https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclasslongptrw
internal enum ClassLongValue
{
	/// <summary>
	/// Retrieves an ATOM value that uniquely identifies the window class.
	/// This is the same atom that the <see cref="WinAPI.RegisterClass"/> function returns.
	/// </summary>
	Atom = -32,
	/// <summary> Retrieves the size, in bytes, of the extra memory associated with the class. </summary>
	CBClassExtra = -20,
	/// <summary>
	/// Retrieves the size, in bytes, of the extra window memory associated with each window in the class.
	/// For information on how to access this memory, see
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptra">GetWindowLongPtr</see>.
	/// </summary>
	CBWindowExtra = -18,
	/// <summary> Retrieves a handle to the background brush associated with the class. </summary>
	HBRBackground = -10,
	/// <summary> Retrieves a handle to the cursor associated with the class. </summary>
	Cursor = -12,
	/// <summary> Retrieves a handle to the icon associated with the class. </summary>
	Icon = -14,
	/// <summary> Retrieves a handle to the small icon associated with the class. </summary>
	IconSmall = -34,
	/// <summary> Retrieves a handle to the module that registered the class. </summary>
	Module = -16,
	/// <summary>
	/// Retrieves the pointer to the menu name string. The string identifies the menu resource associated with the class.
	/// </summary>
	MenuName = -8,
	/// <summary> Retrieves the window-class style bits. </summary>
	Style = -26,
	/// <summary>
	/// Retrieves the address of the window procedure, or a handle representing the address of the window procedure.
	/// You must use the <see cref="WinAPI.CallWindowProc"/> function to call the window procedure.
	/// </summary>
	WindowProc = -24,
}
