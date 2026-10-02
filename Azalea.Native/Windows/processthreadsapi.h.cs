using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentthreadid">Official Documentation</see></summary>
	[LibraryImport(Kernel32Path)]
	public static partial nint GetCurrentThreadId();
}
