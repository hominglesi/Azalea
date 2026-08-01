using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	private const string Kernel32Path = "Kernel32.dll";

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentthreadid">Official Documentation</see></summary>
	[LibraryImport(Kernel32Path)]
	public static partial nint GetCurrentThreadId();
}
