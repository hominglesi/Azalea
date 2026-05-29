using Azalea.Graphics.Colors;
using Azalea.Native.OpenGL;
using Azalea.Platform.Windowing;
using Azalea.Utils;
using System;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer : PlatformRenderer
{
	private readonly PlatformDeviceContext _deviceContext;
	private GLContext _context;

	internal GLRenderer(PlatformDeviceContext deviceContext)
	{
		_deviceContext = deviceContext;
	}

	protected override void Initialize()
	{
		assureGLInitialized();

		_context = GLContext.Create(_deviceContext);
		_context.MakeCurrent();

		var framebuffer = CreateFramebuffer();
		BindFramebuffer(framebuffer);

		var framebufferTexture = CreateTexture2D();
		BindTexture2D(framebufferTexture);

		GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGB, 800, 600, 0, GL.RGB, GL.UNSIGNED_BYTE, IntPtr.Zero);

		BindFramebuffer(null);
	}

	protected override void Update()
	{
		Clear(Rng.Color());
		_context.SwapBuffers();
	}

	private Color? _clearColor = null;

	public override void Clear(Color color)
	{
		if (color != _clearColor)
			GL.ClearColor(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);

		GL.Clear(GL.COLOR_BUFFER_BIT);
	}

	public override Framebuffer CreateFramebuffer()
	{
		uint handle = 0;
		GL.GenFramebuffers(1, ref handle);
		return new GLFramebuffer(this, handle);
	}

	protected override void BindFramebufferImplementation(Framebuffer? framebuffer)
	{
		if (framebuffer is null)
			GL.BindFramebuffer(GL.FRAMEBUFFER, 0);
		else
		{
			if (framebuffer is not GLFramebuffer glFramebuffer)
				throw new ArgumentException("Framebuffer type missmatch!");

			GL.BindFramebuffer(GL.FRAMEBUFFER, glFramebuffer.Handle);
		}
	}

	public override Texture2D CreateTexture2D()
	{
		uint handle = 0;
		GL.GenTextures(1, ref handle);
		return new GLTexture2D(this, handle);
	}

	public override void BindTexture2DImplementation(Texture2D texture2D)
	{
		if (texture2D is not GLTexture2D glTexture2D)
			throw new ArgumentException("Texture2D type missmatch!");

		GL.BindTexture(GL.TEXTURE_2D, glTexture2D.Handle);
	}


}
