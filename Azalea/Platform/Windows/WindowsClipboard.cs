using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal class WindowsClipboard : IClipboard
{
	public string? GetText()
	{
		if (WinAPI.OpenClipboard(IntPtr.Zero) == false)
			return null;

		string? output = null;

		var textHandle = WinAPI.GetClipboardData(13);
		if (textHandle != IntPtr.Zero)
		{
			var lockedHandle = WinAPI.GlobalLock(textHandle);
			if (lockedHandle != IntPtr.Zero)
			{
				output = Marshal.PtrToStringUni(lockedHandle);
				WinAPI.GlobalUnlock(textHandle);
			}
		}
		WinAPI.CloseClipboard();

		return output;
	}

	public bool SetText(string text)
	{
		if (WinAPI.OpenClipboard(IntPtr.Zero) == false)
			return false;

		try
		{
			uint bytes = ((uint)text.Length + 1) * 2;
			uint flags = 0x0002 /* = GMEM_MOVABLE*/ | 0x0040 /* = GMEM_ZEROINIT*/;

			var globalObject = WinAPI.GlobalAlloc(flags, (UIntPtr)bytes);
			if (globalObject == IntPtr.Zero)
				return false;

			try
			{
				var unicodeData = Marshal.StringToHGlobalUni(text);

				try
				{
					var lockedObject = WinAPI.GlobalLock(globalObject);
					if (lockedObject == IntPtr.Zero)
						return false;

					try { WinAPI.CopyMemory(lockedObject, unicodeData, bytes); }
					finally { WinAPI.GlobalUnlock(lockedObject); }

					if (WinAPI.SetClipboardData(13, globalObject) == IntPtr.Zero)
						return false;

					globalObject = IntPtr.Zero;
				}
				finally { Marshal.FreeHGlobal(unicodeData); }
			}
			finally { if (globalObject != IntPtr.Zero) WinAPI.GlobalFree(globalObject); }
		}
		finally { WinAPI.CloseClipboard(); }

		return true;
	}
}
