﻿using Azalea.Graphics;
using Azalea.Numerics;
using Azalea.Platform.Windows.Enums;
using Azalea.Platform.Windows.Enums.RawInput;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

internal static partial class WinAPI
{
	private const string Gdi32Path = "gdi32.dll";
	private const string Kernel32Path = "kernel32.dll";
	private const string User32Path = "user32.dll";

	[DllImport(User32Path, EntryPoint = "AdjustWindowRectEx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AdjustWindowRect(ref WinRectangle rect, WindowStyles style, bool menu, WindowStylesEx exStyle);

	[DllImport(User32Path, EntryPoint = "BringWindowToTop")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BringWindowToTop(IntPtr window);

	[DllImport(User32Path, EntryPoint = "CloseClipboard")]
	public static extern bool CloseClipboard();

	[DllImport(Kernel32Path, EntryPoint = "RtlCopyMemory")]
	public static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

	[DllImport(Gdi32Path, EntryPoint = "CreateBitmap")]
	public static extern IntPtr CreateBitmap(
		int width,
		int height,
		uint planes,
		uint bitCount,
		ref byte data);

	[DllImport(Gdi32Path, EntryPoint = "CreateCompatibleBitmap")]
	public static extern IntPtr CreateCompatibleBitmap(IntPtr deviceContext, int width, int height);

	[DllImport(Kernel32Path, EntryPoint = "CreateFileW")]
	public static extern SafeFileHandle CreateFile(
		[MarshalAs(UnmanagedType.LPWStr)] string fileName,
		[MarshalAs(UnmanagedType.U4)] FileAccess desiredAcces,
		[MarshalAs(UnmanagedType.U4)] FileShare shareMode,
		IntPtr securityAttributes,
		[MarshalAs(UnmanagedType.U4)] CreationDisposition creationDisposition,
		[MarshalAs(UnmanagedType.U4)] FileFlagAttributes flagsAndAttributes,
		IntPtr templateFile);

	public static IntPtr CreateIconFromImage(IntPtr deviceContext, Image image)
	{
		//Windows expects BGRA pixels so we have to swap them
		var swappedBuffer = new byte[image.Data.Length];

		for (int i = 0; i < image.Data.Length; i += 4)
		{
			swappedBuffer[i] = image.Data[i + 2];
			swappedBuffer[i + 1] = image.Data[i + 1];
			swappedBuffer[i + 2] = image.Data[i];
			swappedBuffer[i + 3] = image.Data[i + 3];
		}

		IntPtr color = CreateBitmap(image.Width, image.Height, 1, 32, ref swappedBuffer[0]);
		if (color == IntPtr.Zero)
		{
			Console.WriteLine("Failed to create bitmap");
			return IntPtr.Zero;
		}

		IntPtr mask = CreateCompatibleBitmap(deviceContext, image.Width, image.Height);
		if (mask == IntPtr.Zero)
		{
			Console.WriteLine("Failed to create mask");
			DeleteObject(color);
			return IntPtr.Zero;
		}

		var iconInfo = new IconInfo(true, mask, color);

		var hIcon = CreateIconIndirect(ref iconInfo);
		if (mask == IntPtr.Zero)
		{
			Console.WriteLine("Failed to create icon");
		}
		DeleteObject(color);
		DeleteObject(mask);

		return hIcon;
	}

	[DllImport(User32Path, EntryPoint = "CreateIconIndirect")]
	public static extern IntPtr CreateIconIndirect(ref IconInfo info);

	[DllImport(Gdi32Path, EntryPoint = "ChoosePixelFormat")]
	public static extern int ChoosePixelFormat(IntPtr deviceContext, [In] ref PixelFormatDescriptor descriptor);

	[DllImport(User32Path, EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode)]
	public static extern IntPtr CreateWindow(
		WindowStylesEx stylesEx,
		ushort classAtom,
		string title,
		WindowStyles styles,
		int x,
		int y,
		int width,
		int height,
		IntPtr parent,
		IntPtr menu,
		IntPtr instance,
		IntPtr param);

	[DllImport(User32Path, EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
	public static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

	[DllImport(Gdi32Path, EntryPoint = "DeleteObject")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DeleteObject(IntPtr obj);

	[DllImport(Gdi32Path, EntryPoint = "DescribePixelFormat")]
	public static extern int DescribePixelFormat(IntPtr deviceContext, int pixelFormat, uint bytes, [In, Out] ref PixelFormatDescriptor descriptor);

	[DllImport(User32Path, EntryPoint = "DestroyWindow")]
	public static extern bool DestroyWindow(IntPtr window);

	[DllImport(User32Path, EntryPoint = "DispatchMessageW", CharSet = CharSet.Unicode)]
	public static extern IntPtr DispatchMessage([In] ref Message message);

	[DllImport(User32Path, EntryPoint = "EnableWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnableWindow(IntPtr window, bool enable);

	[DllImport(User32Path, EntryPoint = "FlashWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool FlashWindow(IntPtr window, bool invert);

	[DllImport(User32Path, EntryPoint = "GetClassLongPtrW", CharSet = CharSet.Unicode)]
	public static extern IntPtr GetClassLongPtr(IntPtr window, ClassLongValue index);

	[DllImport(User32Path, EntryPoint = "GetClientRect")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool getClientRect(IntPtr window, out WinRectangle rect);
	public static RectangleInt GetClientRect(IntPtr window)
	{
		getClientRect(window, out var rect);
		return rect;
	}

	[DllImport(User32Path, EntryPoint = "GetClipboardData")]
	public static extern IntPtr GetClipboardData(uint format);

	[DllImport(User32Path, EntryPoint = "GetCursorPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCursorPos(out Vector2Int point);

	[DllImport(User32Path, EntryPoint = "GetDC")]
	public static extern IntPtr GetDC(IntPtr window);

	[DllImport(User32Path, EntryPoint = "GetDesktopWindow")]
	public static extern IntPtr GetDesktopWindow();

	[DllImport(User32Path, EntryPoint = "GetMessageW", CharSet = CharSet.Unicode)]
	private static extern sbyte getMessage(out Message message, IntPtr window, uint wMsgFilterMin, uint wMsgFilterMax);
	public static sbyte GetMessage(out Message message, IntPtr window) => getMessage(out message, window, 0, 0);

	[DllImport(User32Path, EntryPoint = "GetMonitorInfoW", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool getMonitorInfo(IntPtr monitor, ref MonitorInfo info);
	public static MonitorInfo GetMonitorInfo(IntPtr monitor)
	{
		var info = new MonitorInfo();
		if (getMonitorInfo(monitor, ref info) == false)
			Console.WriteLine("GetMonitorInfo failed");

		return info;
	}

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

	[DllImport(User32Path, EntryPoint = "GetWindowRect")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool getWindowRect(IntPtr window, out WinRectangle rect);
	public static RectangleInt GetWindowRect(IntPtr window)
	{
		getWindowRect(window, out var rect);
		return rect;
	}

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
	public static extern bool IsDialogMessage(IntPtr window, [In] ref Message message);

	[DllImport(User32Path, EntryPoint = "LoadCursorW", CharSet = CharSet.Unicode)]
	public static extern IntPtr LoadCursor(IntPtr instance, uint cursorValue);

	[DllImport(User32Path, EntryPoint = "MonitorFromWindow")]
	public static extern IntPtr MonitorFromWindow(IntPtr window, MonitorFromFlags flags);

	[DllImport(User32Path, EntryPoint = "OpenClipboard")]
	public static extern bool OpenClipboard(IntPtr newWindowOwner);

	[DllImport(User32Path, EntryPoint = "PeekMessageW", CharSet = CharSet.Unicode)]
	private static extern sbyte peekMessage(out Message message, IntPtr window, uint minFilter, uint maxFilter, uint remove);
	public static sbyte PeekMessage(out Message message, IntPtr window)
		=> peekMessage(out message, window, 0, 0, 0x0001); // 0x0001 = PM_REMOVE

	[DllImport(User32Path, EntryPoint = "PostQuitMessage")]
	public static extern void PostQuitMessage(int exitCode);

	[DllImport(User32Path, EntryPoint = "RedrawWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RedrawWindow(IntPtr window, WinRectangle? rectangle, IntPtr region, uint flags);

	[DllImport(User32Path, EntryPoint = "RegisterClassExW", CharSet = CharSet.Unicode)]
	public static extern ushort RegisterClass([In] ref WindowClass windowClass);

	[DllImport(User32Path, EntryPoint = "RegisterRawInputDevices", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RegisterRawInputDevices([In] ref RawInputDevice devices, uint count, uint size);

	[DllImport(User32Path, EntryPoint = "ReleaseCapture")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ReleaseCapture();

	[DllImport(User32Path, EntryPoint = "ReleaseDC")]
	public static extern bool ReleaseDC(IntPtr window, IntPtr deviceContext);

	[DllImport(User32Path, EntryPoint = "ScreenToClient")]
	public static extern bool ScreenToClient(IntPtr window, ref Vector2Int point);

	[DllImport(User32Path, EntryPoint = "SendMessage")]
	public static extern IntPtr SendMessage(IntPtr window, WindowMessage message, IntPtr wParam, IntPtr lParam);

	[DllImport(User32Path, EntryPoint = "SetCapture")]
	public static extern IntPtr SetCapture(IntPtr window);

	[DllImport(User32Path, EntryPoint = "SetClipboardData")]
	public static extern IntPtr SetClipboardData(uint format, IntPtr memoryObject);

	[DllImport(User32Path, EntryPoint = "SetFocus")]
	public static extern IntPtr SetFocus(IntPtr window);

	[DllImport(User32Path, EntryPoint = "SetForegroundWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetForegroundWindow(IntPtr window);

	[DllImport(Gdi32Path, EntryPoint = "SetPixelFormat")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetPixelFormat(IntPtr deviceContext, int format, [In] ref PixelFormatDescriptor descriptor);

	[DllImport(User32Path, EntryPoint = "SetWindowLongW", CharSet = CharSet.Unicode)]
	public static extern uint SetWindowLong(IntPtr window, WindowLongValue index, uint newValue);

	[DllImport(User32Path, EntryPoint = "SetWindowPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(
		IntPtr window,
		IntPtr insertAfter,
		int x,
		int y,
		int width,
		int height,
		SetWindowPosFlags flags);

	public static void SetWindowStyle(IntPtr window, WindowStyles style)
	{
		if (SetWindowLong(window, WindowLongValue.Style, (uint)style) == 0)
			Console.WriteLine("Couldn't set window style");
	}

	[DllImport(User32Path, EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowText(IntPtr window, string text);

	[DllImport(User32Path, EntryPoint = "ShowCursor")]
	public static extern int ShowCursor(bool show);

	[DllImport(User32Path, EntryPoint = "ShowWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr window, ShowWindowCommand showCommand);

	[DllImport(Gdi32Path, EntryPoint = "SwapBuffers")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SwapBuffers(IntPtr deviceContext);

	[DllImport(User32Path, EntryPoint = "SystemParametersInfoW", CharSet = CharSet.Unicode)]
	private static extern bool systemParametersInfoRect(
		uint action,
		uint param,
		[Out] out WinRectangle rect,
		uint winIni);

	public static RectangleInt GetSystemWorkArea()
	{
		// 0x0030 = SPI_GETWORKAREA
		systemParametersInfoRect(0x0030, 0, out WinRectangle rect, 0);
		return rect;
	}

	[DllImport(User32Path, EntryPoint = "TranslateMessage")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool TranslateMessage([In] ref Message message);

	[DllImport(User32Path, EntryPoint = "UpdateWindow")]
	public static extern bool UpdateWindow(IntPtr window);

	[DllImport(User32Path, EntryPoint = "WindowFromPoint")]
	public static extern IntPtr WindowFromPoint(Vector2Int point);
}
