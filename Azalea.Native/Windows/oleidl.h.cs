using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	[ComImport]
	[Guid("00000122-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDropTarget
	{
		[PreserveSig]
		int DragEnter([In] IDataObject pDataObj, [In] uint grfKeyState, POINT pt, ref uint pdwEffect);

		[PreserveSig]
		int DragLeave();

		[PreserveSig]
		int DragOver([In] uint grfKeyState, [In] POINT pt, ref uint pdwEffect);

		[PreserveSig]
		int Drop([In] IDataObject pDataObj, [In] uint grfKeyState, [In] POINT pt, ref uint pdwEffect);
	}
}
