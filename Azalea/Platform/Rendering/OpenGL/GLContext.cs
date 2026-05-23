using Azalea.Native.Windows.Win32;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Rendering.OpenGL;
internal class GLContext
{
	public nint Handle { get; init; }
	public PlatformDeviceContext DeviceContext { get; init; }

	private GLContext(nint handle, PlatformDeviceContext deviceContext)
	{
		Handle = handle;
		DeviceContext = deviceContext;
	}

	public static GLContext CreateSimple(PlatformDeviceContext deviceContext)
	{
		if (deviceContext is WindowsDeviceContext winDeviceContext)
		{
			winDeviceContext.SetDefaultPixelFormat();
			var context = Win32.wglCreateContext(winDeviceContext.Handle);

			if (context == nint.Zero)
				throw new Exception($"Could not create context. (Error {Marshal.GetLastWin32Error()})");

			return new GLContext(context, deviceContext);
		}

		throw new NotSupportedException("Device context is not supported");
	}

	private readonly Dictionary<int, GLContext> _activeContexts = [];

	public void MakeCurrent()
	{
		if (DeviceContext is WindowsDeviceContext winDeviceContext)
		{
			if (Win32.wglMakeCurrent(winDeviceContext.Handle, Handle) == false)
				throw new Exception("Could not make context current");

			_activeContexts[Environment.CurrentManagedThreadId] = this;
			return;
		}

		throw new NotSupportedException("Device context is not supported");
	}

	public void SwapBuffers()
	{
		if (DeviceContext is WindowsDeviceContext winDeviceContext)
		{
			Win32.SwapBuffers(winDeviceContext.Handle);
			return;
		}

		throw new NotSupportedException("Device context is not supported");
	}
}
