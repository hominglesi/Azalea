using Azalea.Graphics;
using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Vulkan;
using Azalea.Platform.Windows;
using System;

namespace Azalea.Platform;
internal class DesktopGameHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private Win32Window _window;

	public override IRenderer Renderer { get; }

	public DesktopGameHost(HostPreferences prefs)
	{
		_window = new Win32Window(prefs.WindowTitle, prefs.ClientSize, prefs.WindowState, prefs.WindowVisible)
		{
			//These are fine just being set normaly
			VSync = prefs.VSync,
			Resizable = prefs.WindowResizable
		};

		Renderer = prefs.GraphicsAPI switch
		{
			GraphicsAPI.Vulkan => new VulkanRenderer(_window),
			GraphicsAPI.OpenGL => new GLRenderer(_window),
			_ => throw new Exception($"DesktopGameHost does not support the {prefs.GraphicsAPI} Graphics API")
		};
	}

	public override void CallInitialized()
	{
		GL.Enable(GLCapability.Blend);
		GL.BlendFunc(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		base.CallInitialized();
	}
}
