using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azalea.Native.Windows;

public static partial class Win32
{
	private const string Kernel32Path = "Kernel32.dll";

	public const uint GHND = 0x0042;

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-globalalloc">Official Documentation</see></summary>
	[LibraryImport(Kernel32Path)]
	public static partial nint GlobalAlloc(uint uFlags, nuint dwBytes);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-globalfree">Official Documentation</see></summary>
	[LibraryImport(Kernel32Path)]
	public static partial nint GlobalFree(nint hMem);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-globallock">Official Documentation</see></summary>
	[LibraryImport(Kernel32Path)]
	public static partial nint GlobalLock(nint hMem);

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-globalunlock">Official Documentation</see></summary>
	[LibraryImport(Kernel32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GlobalUnlock(nint hMem);

	public const uint GMEM_FIXED = 0x0000;
	public const uint GMEM_MOVEABLE = 0x0002;
	public const uint GMEM_ZEROINIT = 0x0040;
	public const uint GPTR = 0x0040;
}
