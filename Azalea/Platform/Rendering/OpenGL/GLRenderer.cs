using Azalea.Graphics.Colors;
using Azalea.Platform.Windowing;
using Azalea.Utils;
using System;
using GL = Azalea.Native.OpenGL;

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

		GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGB, 800, 600, 0, GL.GL_RGB, GL.GL_UNSIGNED_BYTE, IntPtr.Zero);

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
			GL.glClearColor(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);

		GL.glClear(GL.GL_COLOR_BUFFER_BIT);
	}

	public override Framebuffer CreateFramebuffer()
	{
		uint handle = 0;
		GLContext.glGenFramebuffers(1, ref handle);
		return new GLFramebuffer(this, handle);
	}

	protected override void BindFramebufferImplementation(Framebuffer? framebuffer)
	{
		if (framebuffer is null)
			GLContext.glBindFramebuffer(GL.GL_FRAMEBUFFER, 0);
		else
		{
			if (framebuffer is not GLFramebuffer glFramebuffer)
				throw new ArgumentException("Framebuffer type missmatch!");

			GLContext.glBindFramebuffer(GL.GL_FRAMEBUFFER, glFramebuffer.Handle);
		}
	}

	public override Texture2D CreateTexture2D()
	{
		uint handle = 0;
		GL.glGenTextures(1, ref handle);
		return new GLTexture2D(this, handle);
	}

	public override void BindTexture2DImplementation(Texture2D texture2D)
	{
		if (texture2D is not GLTexture2D glTexture2D)
			throw new ArgumentException("Texture2D type missmatch!");

		GL.glBindTexture(GL.GL_TEXTURE_2D, glTexture2D.Handle);
	}


}
