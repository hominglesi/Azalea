using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal static partial class WinAPI
{
	private const string XInputPath = "Xinput1_4.dll";

	[DllImport(XInputPath, EntryPoint = "XInputGetState")]
	public static extern int XInputGetState(int index, ref XInputState state);
}
