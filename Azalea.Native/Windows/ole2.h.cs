using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Azalea.Native.Windows;

public static partial class Win32
{
	private const string Ole32Path = "ole32.dll";

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-oleinitialize">Official Documentation</see></summary>
	[LibraryImport(Ole32Path)]
	public static partial uint OleInitialize(nint pvReserved);

#pragma warning disable SYSLIB1054 // Since we aren't in charge of IDropTarget we
	// can't use LibraryImport because it doesn't know how to marshal it

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-registerdragdrop">Official Documentation</see></summary>
	[DllImport(Ole32Path)]
	public static extern uint RegisterDragDrop(nint hwnd, IDropTarget pDropTarget);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-releasestgmedium">Official Documentation</see></summary>
	[DllImport(Ole32Path)]
	public static extern void ReleaseStgMedium(ref STGMEDIUM medium);

#pragma warning restore SYSLIB1054
}
