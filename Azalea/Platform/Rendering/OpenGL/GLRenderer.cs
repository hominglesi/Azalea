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

		IssuePriorityCommand(new TexImage2DCommand(framebufferTexture, GL.TEXTURE_2D, 0, GL.RGB, 800, 600, 0, GL.RGB, GL.UNSIGNED_BYTE, null));
		IssuePriorityCommand(new TexParameteri(framebufferTexture, GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR));
		IssuePriorityCommand(new TexParameteri(framebufferTexture, GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR));
		IssuePriorityCommand(new FramebufferTexture2D(framebuffer, framebufferTexture, GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0, GL.TEXTURE_2D, 0));
	}

	protected override void Update()
	{
		Clear(Rng.Color());
		_context.SwapBuffers();
	}

	internal override void HandleCommand(RenderCommand command)
	{
		switch (command)
		{
			case FramebufferTexture2D(var framebuffer, var texture, var target, var attachment, var textarget, var level):
				GL.BindFramebuffer(target, ((GLFramebuffer)framebuffer).Handle);
				GL.FramebufferTexture2D(target, attachment, textarget, ((GLTexture2D)texture).Handle, 0);
				GL.BindFramebuffer(target, 0);
				break;
			case TexImage2DCommand(var texture, var target, var level, var internalFormat, var width, var height, var border, var format, var type, var pixels):
				GL.BindTexture(target, ((GLTexture2D)texture).Handle);
				if (pixels is null)
					GL.TexImage2D(target, level, internalFormat, width, height, border, format, type, IntPtr.Zero);
				else
					GL.TexImage2D(target, level, internalFormat, width, height, border, format, type, in pixels[0]);
				GL.BindTexture(target, 0);
				break;
			case TexParameteri(var texture, var target, var parameter, var value):
				GL.BindTexture(target, ((GLTexture2D)texture).Handle);
				GL.TexParameteri(target, parameter, value);
				GL.BindTexture(target, 0);
				break;
		}
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
