using Azalea.Inputs;
using Azalea.Platform.Windows.Enums.RawInput;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal class WinRawInputManager
{
	private readonly Dictionary<IntPtr, WinGamepad> _gamepads = new();

	public WinRawInputManager(IntPtr window)
	{
		// Register for gamepad raw input
		var rawInputDevice = new RawInputDevice(1, 5, 0, window);
		WinAPI.RegisterRawInputDevices(ref rawInputDevice, 1, (uint)Marshal.SizeOf<RawInputDevice>());
	}

	public void HandleRawInput(IntPtr rawInputHandle)
	{
		if (getRawInputData(rawInputHandle, out var rawInput, out var rawInputBuffer) == false)
			return;

		try
		{
			var device = rawInput.Header.Device;
			if (_gamepads.ContainsKey(device) == false)
			{
				if (getDeviceInfo(device, out var deviceInfo) == false)
					return;

				// Ignore everything except gamepads
				if (deviceInfo.Type != 2 /* Hid */ ||
					deviceInfo.Hid.UsagePage != 1  /* Generic */ ||
					deviceInfo.Hid.Usage != 5 /* Gamepad */)
					return;

				registerGamepad(rawInput.Header.Device);
				Input.HandleGamepadConnected(_gamepads[device]);
			}

			reportGamepadInput(rawInput, rawInputBuffer, _gamepads[device]);
		}
		finally
		{
			Marshal.FreeHGlobal(rawInputBuffer);
		}
	}

	private static bool getRawInputData(IntPtr rawInputHandle, out RawInput rawInput, out IntPtr rawInputBuffer)
	{
		rawInput = new RawInput();
		rawInputBuffer = IntPtr.Zero;

		try
		{
			uint size = 0;
			uint headerSize = (uint)Marshal.SizeOf(typeof(RawInputHeader));
			WinAPI.GetRawInputData(rawInputHandle, RawInputCommand.Input, IntPtr.Zero, ref size, headerSize);

			rawInputBuffer = Marshal.AllocHGlobal((int)size);
			var result = WinAPI.GetRawInputData(rawInputHandle, RawInputCommand.Input, rawInputBuffer, ref size, headerSize);

			if (result != size)
			{
				Marshal.FreeHGlobal(rawInputBuffer);
				rawInputBuffer = IntPtr.Zero;
				return false;
			}

			rawInput = Marshal.PtrToStructure<RawInput>(rawInputBuffer);
			return true;
		}
		catch
		{
			if (rawInputBuffer != IntPtr.Zero)
				Marshal.FreeHGlobal(rawInputBuffer);

			return false;
		}
	}

	private static bool getDeviceInfo(IntPtr device, out RawInputDeviceInfo info)
	{
		info = new RawInputDeviceInfo();
		IntPtr infoBuffer = IntPtr.Zero;

		try
		{
			uint infoSize = (uint)Marshal.SizeOf(typeof(RawInputDeviceInfo));
			infoBuffer = Marshal.AllocHGlobal((int)infoSize);

			int result = WinAPI.GetRawInputDeviceInfo(device, RawInputDeviceInfoType.DeviceInfo, infoBuffer, ref infoSize);

			if (result < 0)
				return false;

			info = Marshal.PtrToStructure<RawInputDeviceInfo>(infoBuffer);
			return true;
		}
		catch
		{
			return false;
		}
		finally
		{
			Marshal.FreeHGlobal(infoBuffer);
		}
	}

	private void registerGamepad(IntPtr device)
	{
		var preparsedData = getPreparsedData(device);

		if (preparsedData == IntPtr.Zero)
			return;

		var capabilities = new HidPCaps();
		var status = WinAPI.HidP_GetCaps(preparsedData, ref capabilities);

		if (status != HidStatus.Success)
			return;

		HidPButtonCaps[] buttonCapabilities = new HidPButtonCaps[capabilities.NumberInputButtonCaps];
		if (capabilities.NumberInputButtonCaps > 0)
		{
			ushort buttonCapabilitiesCount = capabilities.NumberInputButtonCaps;
			status = WinAPI.HidP_GetButtonCaps(HidPReportType.Input, buttonCapabilities, ref buttonCapabilitiesCount, preparsedData);

			if (status != HidStatus.Success || buttonCapabilitiesCount != capabilities.NumberInputButtonCaps)
				return;
		}

		HidPValueCaps[] valueCapabilities = new HidPValueCaps[capabilities.NumberInputValueCaps];
		if (capabilities.NumberInputValueCaps > 0)
		{
			ushort valueCapabilitiesCount = capabilities.NumberInputValueCaps;
			status = WinAPI.HidP_GetValueCaps(HidPReportType.Input, valueCapabilities, ref valueCapabilitiesCount, preparsedData);

			if (status != HidStatus.Success || valueCapabilitiesCount != capabilities.NumberInputValueCaps)
				return;
		}

		var gamepad = new WinGamepad(preparsedData, capabilities, buttonCapabilities, valueCapabilities);
		_gamepads.Add(device, gamepad);
	}

	private static IntPtr getPreparsedData(IntPtr device)
	{
		uint size = 0;
		int result = WinAPI.GetRawInputDeviceInfo(device, RawInputDeviceInfoType.PreparsedData, IntPtr.Zero, ref size);

		if (result != 0)
			return IntPtr.Zero;

		var preparsedData = Marshal.AllocHGlobal((int)size);
		result = WinAPI.GetRawInputDeviceInfo(device, RawInputDeviceInfoType.PreparsedData, preparsedData, ref size);

		if (result < 0)
			return IntPtr.Zero;

		return preparsedData;
	}

	private void reportGamepadInput(RawInput input, IntPtr rawInputBuffer, WinGamepad gamepad)
	{
		var inputReport = new byte[input.HID.SizeHid];

		for (int i = 0; i < input.HID.Count; i++)
		{
			unsafe
			{
				byte* source = (byte*)rawInputBuffer;
				source += sizeof(RawInputHeader) + sizeof(RawHID) + (input.HID.SizeHid * i);

				Marshal.Copy(new IntPtr(source), inputReport, 0, (int)input.HID.SizeHid);
			}

			reportGamepadButtons(gamepad, inputReport);
			reportGamepadValues(gamepad, inputReport);
		}
	}

	private readonly List<HidUsageAndPage> _lastUsages = new();

	private void reportGamepadButtons(WinGamepad gamepad, byte[] inputReport)
	{
		uint usageCount = 0;

		var usages = Array.Empty<HidUsageAndPage>();
		var status = WinAPI.HidP_GetUsagesEx(HidPReportType.Input, 0, usages, ref usageCount, gamepad.PreparsedData, inputReport, (uint)inputReport.Length);

		if (status == HidStatus.BufferTooSmall)
		{
			usages = new HidUsageAndPage[usageCount];
			status = WinAPI.HidP_GetUsagesEx(HidPReportType.Input, 0, usages, ref usageCount, gamepad.PreparsedData, inputReport, (uint)inputReport.Length);

			if (status != HidStatus.Success)
				return;
		}

		foreach (var lastUsage in _lastUsages)
		{
			bool stillExists = false;
			foreach (var usage in usages)
			{
				if (lastUsage == usage)
				{
					stillExists = true;
					break;
				}
			}

			if (stillExists == false)
				gamepad.Buttons[lastUsage.Usage].SetState(false);
		}

		_lastUsages.Clear();
		foreach (var usage in usages)
		{
			if (gamepad.Buttons.ContainsKey(usage.Usage) == false)
				gamepad.Buttons.Add(usage.Usage, new ButtonState());

			var button = gamepad.Buttons[usage.Usage];

			if (button.Released)
				button.SetState(true);

			_lastUsages.Add(usage);
		}
	}

	private void reportGamepadValues(WinGamepad gamepad, byte[] inputReport)
	{
		foreach (var valueCapability in gamepad.ValueCapabilities)
		{
			uint usageValue = 0;
			ushort usage = valueCapability.NotRange.Usage;

			var status = WinAPI.HidP_GetUsageValue(HidPReportType.Input, valueCapability.UsagePage,
				valueCapability.LinkCollection, usage, ref usageValue,
				gamepad.PreparsedData, inputReport, (uint)inputReport.Length);

			if (status != HidStatus.Success)
				continue;

			if (usage == 0x39 /* HatSwitch (DPad) */)
			{
				float vertical = 0;
				float horizontal = 0;

				if (usageValue != 0)
				{
					if (usageValue == 4 || usageValue == 5 || usageValue == 6)
						vertical = 1;
					else if (usageValue == 1 || usageValue == 2 || usageValue == 8)
						vertical = -1;

					if (usageValue == 2 || usageValue == 3 || usageValue == 4)
						horizontal = 1;
					else if (usageValue == 6 || usageValue == 7 || usageValue == 8)
						horizontal = -1;
				}

				gamepad.DPad.SetDirection(horizontal, vertical);
				continue;
			}

			var scaledValue = (usageValue / (float)short.MaxValue) - 1;

			var absoluteScaledValue = Math.Abs(scaledValue);
			if (absoluteScaledValue < 0.01)
				scaledValue = 0;
			else if (absoluteScaledValue > 1)
				scaledValue = Math.Sign(scaledValue);

			if (usage == 0x30 /* Left X */)
				gamepad.LeftAnalogStick.Horizontal = scaledValue;
			else if (usage == 0x31 /* Left Y */)
				gamepad.LeftAnalogStick.Vertical = scaledValue;
			else if (usage == 0x33 /* Right X */)
				gamepad.RightAnalogStick.Horizontal = scaledValue;
			else if (usage == 0x34 /* Right Y */)
				gamepad.RightAnalogStick.Vertical = scaledValue;
		}
	}
}
