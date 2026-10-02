using Azalea.Native.Windows;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal class WindowsClipboard : IClipboard
{
	public string? GetText()
	{
		if (Win32.OpenClipboard(IntPtr.Zero) == false)
			return null;

		string? output = null;

		var textHandle = Win32.GetClipboardData(13);
		if (textHandle != IntPtr.Zero)
		{
			var lockedHandle = Win32.GlobalLock(textHandle);
			if (lockedHandle != IntPtr.Zero)
			{
				output = Marshal.PtrToStringUni(lockedHandle);
				Win32.GlobalUnlock(textHandle);
			}
		}
		Win32.CloseClipboard();

		return output;
	}

	public bool SetText(string text)
	{
		if (Win32.OpenClipboard(IntPtr.Zero) == false)
			return false;

		try
		{
			if (Win32.EmptyClipboard() == false)
				throw new Exception($"Couldn't empty clipboard (error: {Marshal.GetLastWin32Error()})");

			uint bytes = ((uint)text.Length + 1) * sizeof(char);

			var globalObject = Win32.GlobalAlloc(Win32.GMEM_MOVEABLE, bytes);
			if (globalObject == IntPtr.Zero)
				throw new Exception($"Couldn't allocate global object (error: {Marshal.GetLastWin32Error()})");

			try
			{
				var lockedObject = Win32.GlobalLock(globalObject);
				if (lockedObject == IntPtr.Zero)
					throw new Exception($"Couldn't lock global object (error: {Marshal.GetLastWin32Error()})");

				try
				{
					unsafe
					{
						var destination = new Span<char>((void*)lockedObject, text.Length + 1);
						text.AsSpan().CopyTo(destination);
						destination[text.Length] = '\0';
					}
				}
				finally { Win32.GlobalUnlock(lockedObject); }

				if (Win32.SetClipboardData(13, globalObject) == IntPtr.Zero)
					return false;

				globalObject = IntPtr.Zero;
			}
			finally { if (globalObject != IntPtr.Zero) Win32.GlobalFree(globalObject); }
		}
		finally { Win32.CloseClipboard(); }

		return true;
	}
}
