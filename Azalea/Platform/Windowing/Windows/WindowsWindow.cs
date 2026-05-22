using Azalea.Native.Windows.Win32;
using Azalea.Platform.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windowing.Windows;
internal class WindowsWindow : PlatformWindow
{
	private static int _nextClassId = 0;
	private Win32.WNDPROC? _windowProcedure;

	public override string PlatformType => "Windows";
	public ushort ClassAtom { get; private set; }

	protected override void Initialize()
	{
		var processHandle = Process.GetCurrentProcess().Handle;
		_windowProcedure = windowProcedure;

		var classNamePtr = Marshal.StringToHGlobalUni("Azalea Window " + _nextClassId++);
		var winProcPtr = Marshal.GetFunctionPointerForDelegate(_windowProcedure);

		var wndClass = new Win32.WNDCLASSEXW
		{
			lpszClassName = classNamePtr,
			hInstance = processHandle,
			lpfnWndProc = winProcPtr,
			style = Win32.WNDCLASSEXW.ClassStyles.OWNDC,
			hCursor = WinAPI.LoadCursor(nint.Zero, 32512)
		};

		ClassAtom = Win32.RegisterClassExW(ref wndClass);

		Marshal.FreeHGlobal(classNamePtr);
	}

	protected override void Update()
	{

	}

	private nint windowProcedure(nint hWnd, uint uMsg, nint wParam, nint lParam)
	{
		return nint.Zero;
	}
}
