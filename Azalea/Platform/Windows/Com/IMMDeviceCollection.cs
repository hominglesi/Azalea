using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows.Com;

[ComImport]
[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceCollection
{
	[PreserveSig]
	int GetCount(ref int deviceCount);

	[PreserveSig]
	int Item(int deviceIndex, [Out] out IMMDevice device);
}
