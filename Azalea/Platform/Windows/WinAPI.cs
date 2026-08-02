using Azalea.Native.Windows;
using Azalea.Numerics;
using Azalea.Platform.Windows.Com;
using Azalea.Platform.Windows.Enums;
using Azalea.Platform.Windows.Enums.RawInput;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Azalea.Platform.Windows;

internal static partial class WinAPI
{
	private const string Gdi32Path = "gdi32.dll";
	private const string Kernel32Path = "kernel32.dll";
	private const string Ole32Path = "ole32.dll";
	private const string Shell32Path = "shell32.dll";
	private const string User32Path = "user32.dll";

	[DllImport(User32Path, EntryPoint = "CloseClipboard")]
	public static extern bool CloseClipboard();

	[DllImport(Kernel32Path, EntryPoint = "RtlCopyMemory")]
	public static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

	[DllImport(Kernel32Path, EntryPoint = "CreateFileW")]
	public static extern SafeFileHandle CreateFile(
		[MarshalAs(UnmanagedType.LPWStr)] string fileName,
		[MarshalAs(UnmanagedType.U4)] FileAccess desiredAcces,
		[MarshalAs(UnmanagedType.U4)] FileShare shareMode,
		IntPtr securityAttributes,
		[MarshalAs(UnmanagedType.U4)] CreationDisposition creationDisposition,
		[MarshalAs(UnmanagedType.U4)] FileFlagAttributes flagsAndAttributes,
		IntPtr templateFile);

	[DllImport(User32Path, EntryPoint = "DestroyWindow")]
	public static extern bool DestroyWindow(IntPtr window);

	[DllImport(Shell32Path, EntryPoint = "DragAcceptFiles")]
	public static extern void DragAcceptFiles(IntPtr window, bool accept);

	[DllImport(Shell32Path, EntryPoint = "DragQueryFileW", CharSet = CharSet.Unicode)]
	public static extern int DragQueryFile(IntPtr drop, uint file, [Out] StringBuilder fileName, uint fileNameLength);

	[DllImport(User32Path, EntryPoint = "EnableWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnableWindow(IntPtr window, bool enable);

	[DllImport(User32Path, EntryPoint = "GetClassLongPtrW", CharSet = CharSet.Unicode)]
	public static extern IntPtr GetClassLongPtr(IntPtr window, ClassLongValue index);

	[DllImport(User32Path, EntryPoint = "GetClipboardData")]
	public static extern IntPtr GetClipboardData(uint format);

	[DllImport(User32Path, EntryPoint = "GetCursorPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCursorPos(out Vector2Int point);

	[DllImport(User32Path, EntryPoint = "GetDesktopWindow")]
	public static extern IntPtr GetDesktopWindow();

	[DllImport(User32Path, EntryPoint = "GetMessageW", CharSet = CharSet.Unicode)]
	private static extern sbyte getMessage(out Win32.MSG message, IntPtr window, uint wMsgFilterMin, uint wMsgFilterMax);
	public static sbyte GetMessage(out Win32.MSG message, IntPtr window) => getMessage(out message, window, 0, 0);

	[DllImport(User32Path, EntryPoint = "GetRawInputData")]
	public static extern uint GetRawInputData(IntPtr rawInput, RawInputCommand command, IntPtr data, ref uint dataSize, uint headerSize);

	[DllImport(User32Path, EntryPoint = "GetRawInputDeviceInfoW")]
	public static extern int GetRawInputDeviceInfo(IntPtr device, RawInputDeviceInfoType command, IntPtr data, ref uint size);

	[DllImport(User32Path, EntryPoint = "GetSystemMetrics")]
	public static extern int GetSystemMetrics(SystemMetric metric);

	[DllImport(User32Path, EntryPoint = "GetWindow")]
	public static extern IntPtr GetWindow(IntPtr window, uint relation);

	[DllImport(User32Path, EntryPoint = "GetWindowLongW", CharSet = CharSet.Unicode)]
	public static extern int GetWindowLong(IntPtr window, int index);

	[DllImport(Kernel32Path, EntryPoint = "GlobalAlloc")]
	public static extern IntPtr GlobalAlloc(uint flags, UIntPtr bytes);

	[DllImport(Kernel32Path, EntryPoint = "GlobalFree")]
	public static extern IntPtr GlobalFree(IntPtr memoryObject);

	[DllImport(Kernel32Path, EntryPoint = "GlobalLock")]
	public static extern IntPtr GlobalLock(IntPtr memoryObject);

	[DllImport(Kernel32Path, EntryPoint = "GlobalUnlock")]
	public static extern bool GlobalUnlock(IntPtr memoryObject);

	[DllImport(User32Path, EntryPoint = "IsDialogMessageW", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool IsDialogMessage(IntPtr window, [In] ref Win32.MSG message);

	[DllImport(User32Path, EntryPoint = "LoadCursorW", CharSet = CharSet.Unicode)]
	public static extern IntPtr LoadCursor(IntPtr instance, uint cursorValue);

	[DllImport(Ole32Path, EntryPoint = "OleInitialize")]
	public static extern uint OleInitialize(nint reserved);

	[DllImport(User32Path, EntryPoint = "OpenClipboard")]
	public static extern bool OpenClipboard(IntPtr newWindowOwner);

	[DllImport(User32Path, EntryPoint = "PostQuitMessage")]
	public static extern void PostQuitMessage(int exitCode);

	[DllImport(User32Path, EntryPoint = "RedrawWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RedrawWindow(IntPtr window, Win32.RECT? rectangle, IntPtr region, uint flags);

	[DllImport(Ole32Path, EntryPoint = "RegisterDragDrop")]
	public static extern uint RegisterDragDrop(IntPtr window, IDropTarget dropTarget);

	[DllImport(User32Path, EntryPoint = "RegisterRawInputDevices", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RegisterRawInputDevices([In] ref RawInputDevice devices, uint count, uint size);

	[DllImport(User32Path, EntryPoint = "ReleaseCapture")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ReleaseCapture();

	[DllImport(User32Path, EntryPoint = "ReleaseDC")]
	public static extern bool ReleaseDC(IntPtr window, IntPtr deviceContext);

	[DllImport(Ole32Path, EntryPoint = "ReleaseStgMedium")]
	public static extern bool ReleaseStgMedium(ref STGMEDIUM medium);

	[DllImport(User32Path, EntryPoint = "ScreenToClient")]
	public static extern bool ScreenToClient(IntPtr window, ref Vector2Int point);

	[DllImport(User32Path, EntryPoint = "SetCapture")]
	public static extern IntPtr SetCapture(IntPtr window);

	[DllImport(User32Path, EntryPoint = "SetClipboardData")]
	public static extern IntPtr SetClipboardData(uint format, IntPtr memoryObject);

	[DllImport(User32Path, EntryPoint = "SystemParametersInfoW", CharSet = CharSet.Unicode)]
	private static extern bool systemParametersInfoRect(
		uint action,
		uint param,
		[Out] out Win32.RECT rect,
		uint winIni);

	public static RectangleInt GetSystemWorkArea()
	{
		// 0x0030 = SPI_GETWORKAREA
		systemParametersInfoRect(0x0030, 0, out Win32.RECT rect, 0);
		return (RectangleInt)rect;
	}

	[DllImport(User32Path, EntryPoint = "UpdateWindow")]
	public static extern bool UpdateWindow(IntPtr window);

	[DllImport(User32Path, EntryPoint = "WindowFromPoint")]
	public static extern IntPtr WindowFromPoint(Vector2Int point);
}
