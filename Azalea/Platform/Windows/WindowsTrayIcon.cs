using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Native.Windows;
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

		IntPtr iconHandle = Win32.CreateIconFromPixelArray(window.DeviceContext, icon.Width, icon.Height, icon.Data);

		var nid = new Win32.NOTIFYICONDATAW
		{
			hWnd = window.Handle,
			uID = Handle,
			uFlags = Win32.NotifyIconFlag.MESSAGE | Win32.NotifyIconFlag.ICON | Win32.NotifyIconFlag.TIP,
			uCallbackMessage = Win32.WindowMessage.AZ_TRAYICON,
			hIcon = iconHandle,
			szTip = iconName
		};

		Win32.Shell_NotifyIconW(Win32.NotifyIconMessage.ADD, ref nid);
	}

	public void Destroy()
	{
		_owningWindow.RemoveTrayIcon(this);

		var nid = new Win32.NOTIFYICONDATAW();
		nid.cbSize = Marshal.SizeOf(nid);
		nid.hWnd = _owningWindow.Handle;
		nid.uID = Handle;

		Win32.Shell_NotifyIconW(Win32.NotifyIconMessage.DELETE, ref nid);
	}

	internal void InvokeClick(MouseButton button)
		=> OnClick?.Invoke(button);

	internal void InvokeDoubleClick(MouseButton button)
		=> OnDoubleClick?.Invoke(button);
}
