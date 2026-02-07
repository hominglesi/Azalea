using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Azalea.Platform.Windows.ComInterfaces;

[ComImport]
[Guid("00000122-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IDropTarget
{
	[PreserveSig]
	int DragEnter(nint dataObject, uint keyState, Vector2Int point, ref uint effect);

	[PreserveSig]
	int DragOver(uint keyState, Vector2Int point, ref uint effect);

	[PreserveSig]
	int DragLeave();

	[PreserveSig]
	int Drop(IDataObject dataObject, uint keyState, Vector2Int point, ref uint effect);
}
