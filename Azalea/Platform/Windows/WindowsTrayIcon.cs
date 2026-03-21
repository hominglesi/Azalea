using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Platform.Windows.Structs;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

internal unsafe partial class WindowsTrayIcon : ITrayIcon
{
	private static uint _nextID = 1;

	public readonly uint Handle;
	private readonly Win32Window _owningWindow;

	public Action<MouseButton>? OnClick { get; set; }
	public Action<MouseButton>? OnDoubleClick { get; set; }

	public WindowsTrayIcon(Win32Window window, string iconName, Image icon)
	{
		Handle = _nextID++;
		_owningWindow = window;

		IntPtr iconHandle = WinAPI.CreateIconFromImage(window.DeviceContext, icon);

		const uint NIF_MESSAGE = 0x01, NIF_ICON = 0x02, NIF_TIP = 0x04;

		var nid = new NOTIFYICONDATA();
		nid.cbSize = Marshal.SizeOf(nid);
		nid.hWnd = window.Handle;
		nid.uID = Handle;
		nid.uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
		nid.uCallbackMessage = WindowMessage.TrayIcon;
		nid.hIcon = iconHandle;
		nid.szTip = iconName;

		const uint NIM_ADD = 0x00;
		Shell_NotifyIconW(NIM_ADD, ref nid);
	}

	public void Destroy()
	{
		_owningWindow.RemoveTrayIcon(this);

		var nid = new NOTIFYICONDATA();
		nid.cbSize = Marshal.SizeOf(nid);
		nid.hWnd = _owningWindow.Handle;
		nid.uID = Handle;

		const uint NIM_DELETE = 0x02;
		Shell_NotifyIconW(NIM_DELETE, ref nid);
	}

	internal void InvokeClick(MouseButton button)
		=> OnClick?.Invoke(button);

	internal void InvokeDoubleClick(MouseButton button)
		=> OnDoubleClick?.Invoke(button);

	[LibraryImport("shell32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool Shell_NotifyIconW(uint dwMessage, ref NOTIFYICONDATA data);
}
