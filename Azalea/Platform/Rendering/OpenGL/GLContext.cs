using Azalea.Native.OpenGL;
using Azalea.Native.Windows;
using Azalea.Platform.Rendering.OpenGL.LoadingContext;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Rendering.OpenGL;
internal class GLContext
{
	public nint Handle { get; init; }
	public IPlatformDeviceContext DeviceContext { get; init; }

	private GLContext(nint handle, IPlatformDeviceContext deviceContext)
	{
		Handle = handle;
		DeviceContext = deviceContext;
	}

	public static GLContext CreateSimple(IPlatformDeviceContext deviceContext)
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

	public static GLContext Create(IPlatformDeviceContext deviceContext, GLLoadingContext? shareContext = null)
	{
		if (deviceContext is WindowsDeviceContext winDeviceContext)
		{
			SetPixelFormat(winDeviceContext);

			var openGLAttribs = new int[]
			{
				GL.WGL_CONTEXT_MAJOR_VERSION_ARB, 3,
				GL.WGL_CONTEXT_MINOR_VERSION_ARB, 3,
				GL.WGL_CONTEXT_PROFILE_MASK_ARB, GL.WGL_CONTEXT_CORE_PROFILE_BIT_ARB,
				0, 0
			};

			shareContext?.ReleaseContext().Await();

			var shareContextHandle = shareContext is null ? nint.Zero : shareContext.Handle;

			var context = GL.wglCreateContextAttribsARB(winDeviceContext.Handle, shareContextHandle, in openGLAttribs[0]);
			if (context == nint.Zero)
				throw new Exception($"Could not create context. (Error {Marshal.GetLastWin32Error()})");

			shareContext?.RebindContext().Await();

			return new GLContext(context, deviceContext);
		}

		throw new NotSupportedException("Device context is not supported");
	}

	private readonly Dictionary<int, GLContext?> _activeContexts = [];

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

	public void Release()
	{
		if (DeviceContext is WindowsDeviceContext winDeviceContext)
		{
			if (Win32.wglMakeCurrent(winDeviceContext.Handle, 0) == false)
				throw new Exception("Could not release context");

			_activeContexts[Environment.CurrentManagedThreadId] = null;
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

	public static void SetPixelFormat(IPlatformDeviceContext deviceContext)
	{
		assertDynamicFunctionsLoaded();

		if (deviceContext is WindowsDeviceContext winDeviceContext)
		{
			var pixelFormatAttribs = new int[]
			{
				GL.WGL_DRAW_TO_WINDOW_ARB, 1,
				GL.WGL_SUPPORT_OPENGL_ARB, 1,
				GL.WGL_DOUBLE_BUFFER_ARB, 1,
				GL.WGL_ACCELERATION_ARB, GL.WGL_FULL_ACCELERATION_ARB,
				GL.WGL_PIXEL_TYPE_ARB, GL.WGL_TYPE_RGBA_ARB,
				GL.WGL_COLOR_BITS_ARB, 32,
				GL.WGL_DEPTH_BITS_ARB, 24,
				GL.WGL_STENCIL_BITS_ARB, 8,
				GL.WGL_SWAP_METHOD_ARB, GL.WGL_SWAP_COPY_ARB,
				0
			};

			int pixelFormat = 0;
			uint formatCount = 0;
			GL.wglChoosePixelFormatARB(winDeviceContext.Handle, in pixelFormatAttribs[0], IntPtr.Zero, 1, ref pixelFormat, ref formatCount);

			Win32.PIXELFORMATDESCRIPTOR pixelFormatDescriptor = default;
			Win32.DescribePixelFormat(winDeviceContext.Handle, pixelFormat, pixelFormatDescriptor.nSize, ref pixelFormatDescriptor);
			Win32.SetPixelFormat(winDeviceContext.Handle, pixelFormat, in pixelFormatDescriptor);
			return;
		}

		throw new NotSupportedException("Device context is not supported");
	}

	public nint GetProcAddress(string functionName)
	{
		if (DeviceContext is WindowsDeviceContext)
			return Win32.wglGetProcAddress(functionName);

		throw new NotSupportedException("Device context is not supported");
	}

	private static void assertDynamicFunctionsLoaded()
	{
		if (GL.DynamicFunctionsLoaded == false)
			throw new Exception("Dynamic functions haven't been loaded!");
	}
}
