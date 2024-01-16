using System;

namespace Azalea.Platform.Windows;


[Flags]
//Mappings from https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputdevice
internal enum RawInputDeviceFlags : uint
{
	/// <summary>
	/// If set, this removes the top level collection from the inclusion list.
	/// This tells the operating system to stop reading from a device which matches the top level collection.
	/// </summary>
	Remove = 0x00000001,
	/// <summary>
	/// If set, this specifies the top level collections to exclude when reading a complete usage page.
	/// This flag only affects a TLC whose usage page is already specified with <see cref="PageOnly"/>.
	/// </summary>
	Exclude = 0x00000010,
	/// <summary>
	/// If set, this specifies all devices whose top level collection is from the specified <see cref="RawInputDevice.UsagePage"/>.
	/// Note that <see cref="RawInputDevice.Usage"/> must be zero.
	/// To exclude a particular top level collection, use <see cref="Exclude"/>.
	/// </summary>
	PageOnly = 0x00000020,
	/// <summary>
	/// If set, this prevents any devices specified by <see cref="RawInputDevice.UsagePage"/>
	/// or <see cref="RawInputDevice.Usage"/> from generating
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/inputdev/mouse-input-notifications">legacy messages</see> .
	/// This is only for the mouse and keyboard. See Remarks.
	/// </summary>
	NoLegacy = 0x00000030,
	/// <summary>
	/// If set, this enables the caller to receive the input even when the caller is not in the foreground.
	/// Note that <see cref="RawInputDevice.WindowTarget"/> must be specified.
	/// </summary>
	InputSink = 0x00000100,
	/// <summary>
	/// If set, the mouse button click does not activate the other window. <see cref="CaptureMouse"/> can be specified
	/// only if <see cref="NoLegacy"/> is specified for a mouse device.
	/// </summary>
	CaptureMouse = 0x00000200,
	/// <summary>
	/// If set, the application-defined keyboard device hotkeys are not handled.
	/// However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled.
	/// By default, all keyboard hotkeys are handled. <see cref="NoHotkeys"/> can be specified
	/// even if <see cref="NoLegacy"/> is not specified and <see cref="RawInputDevice.WindowTarget"/> is NULL.
	/// </summary>
	NoHotkeys = 0x00000200,
	/// <summary>
	/// If set, the application command keys are handled.
	/// <see cref="AppKeys"/> can be specified only if <see cref="NoLegacy"/> is specified for a keyboard device.
	/// </summary>
	AppKeys = 0x00000400,
	/// <summary>
	/// If set, this enables the caller to receive input in the background only if the foreground application
	/// does not process it. In other words, if the foreground application is not registered for raw input,
	/// then the background application that is registered will receive the input.
	/// Windows XP: This flag is not supported until Windows Vista
	/// </summary>
	ExInputSink = 0x00001000,
	/// <summary>
	/// If set, this enables the caller to receive <see cref="WindowMessage.InputDeviceChange"/> notifications for
	/// device arrival and device removal.
	/// Windows XP: This flag is not supported until Windows Vista
	/// </summary>
	DevNotify = 0x00002000
}
