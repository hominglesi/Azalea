using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;

public static partial class Win32
{
	private const string XInputPath = "Xinput1_4.dll";

	[StructLayout(LayoutKind.Sequential)]
	public struct XINPUT_STATE
	{
		public int dwPacketNumber;
		public XINPUT_GAMEPAD Gamepad;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct XINPUT_GAMEPAD
	{
		public ushort wButtons;
		public byte bLeftTrigger;
		public byte bRightTrigger;
		public short sThumbLX;
		public short sThumbLY;
		public short sThumbRX;
		public short sThumbRY;
	}

	/// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetstate">Official Documentation</see></summary>
	[LibraryImport(XInputPath)]
	public static partial int XInputGetState(int index, ref XINPUT_STATE state);
}
