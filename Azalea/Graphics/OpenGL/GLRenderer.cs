using Azalea.Debugging;
using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Batches;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.OpenGL.Textures;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Numerics;
using Azalea.Platform;
using System;

namespace Azalea.Graphics.OpenGL;
internal class GLRenderer : Renderer
{
	private GLFramebuffer _framebuffer;
	private GLFramebuffer _intermediateFramebuffer;
	private uint _screenTexture;
	private uint _screenVertexArray;

	private float[] quadVertices = new float[] {
		-1.0f,  1.0f,  0.0f, 1.0f,
		-1.0f, -1.0f,  0.0f, 0.0f,
		 1.0f, -1.0f,  1.0f, 0.0f,

		-1.0f,  1.0f,  0.0f, 1.0f,
		 1.0f, -1.0f,  1.0f, 0.0f,
		 1.0f,  1.0f,  1.0f, 1.0f
	};

	public GLRenderer(IWindow window)
		: base(window) { }

	protected internal override void Initialize()
	{
		base.Initialize();

		var screenWidth = Window.ClientSize.X;
		var screenHeight = Window.ClientSize.Y;

		_screenVertexArray = GL.GenVertexArray();
		var screenVertexBuffer = GL.GenBuffer();
		GL.BindVertexArray(_screenVertexArray);
		GL.BindBuffer(GLBufferType.Array, screenVertexBuffer);
		GL.BufferData(GLBufferType.Array, quadVertices, GLUsageHint.StaticDraw);
		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, GLDataType.Float, false, 4 * sizeof(float), 0);
		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, GLDataType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

		_framebuffer = new GLFramebuffer();
		_framebuffer.Bind();

		var framebufferTexture = GL.GenTexture();
		GL.BindTexture(GLTextureType.Texture2DMultisample, framebufferTexture);
		GL.TexImage2DMultisample(GLTextureType.Texture2DMultisample, 4, GLColorFormat.RGB, screenWidth, screenHeight, true);
		GL.BindTexture(GLTextureType.Texture2DMultisample, 0);
		GL.FramebufferTexture2D(GLBufferType.Framebuffer, GLAttachment.Color0, GLTextureType.Texture2DMultisample, framebufferTexture, 0);

		var renderBuffer = GL.GenRenderbuffer();
		GL.BindRenderbuffer(GLBufferType.Renderbuffer, renderBuffer);
		GL.RenderbufferStorageMultisample(GLBufferType.Renderbuffer, 4, GLColorFormat.Depth24Stencil8, screenWidth, screenHeight);
		GL.BindRenderbuffer(GLBufferType.Renderbuffer, 0);
		GL.FramebufferRenderbuffer(GLBufferType.Framebuffer, GLAttachment.DepthStencil, GLBufferType.Renderbuffer, renderBuffer);

		if (GL.CheckFramebufferStatus(GLBufferType.Framebuffer) != GLFramebufferStatus.Complete)
			Console.WriteLine("Couldn't create framebuffer.");

		_framebuffer.Unbind();

		_intermediateFramebuffer = new GLFramebuffer();
		_intermediateFramebuffer.Bind();

		_screenTexture = GL.GenTexture();
		GL.BindTexture(GLTextureType.Texture2D, _screenTexture);
		GL.TexImage2D(GLTextureType.Texture2D, 0, GLColorFormat.RGB, screenWidth, screenHeight, 0, GLColorFormat.RGB, GLDataType.UnsignedByte, null);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MinFilter, (int)GLFunction.Linear);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MagFilter, (int)GLFunction.Linear);
		GL.FramebufferTexture2D(GLBufferType.Framebuffer, GLAttachment.Color0, GLTextureType.Texture2D, _screenTexture, 0);

		_intermediateFramebuffer.Unbind();
	}

	internal override void BeginFrame()
	{
		base.BeginFrame();

		_framebuffer.Bind();
		GL.ClearColor(ClearColor);
		GL.Clear(GLBufferBit.Color | GLBufferBit.Depth);
		GL.Enable(GLCapability.Blend);
	}

	internal override void FinishFrame()
	{
		base.FinishFrame();

		var screenWidth = Window.ClientSize.X;
		var screenHeight = Window.ClientSize.Y;

		GL.BindFramebuffer(GLBufferType.ReadFramebuffer, _framebuffer.Handle);
		GL.BindFramebuffer(GLBufferType.DrawFramebuffer, _intermediateFramebuffer.Handle);
		GL.BlitFramebuffer(0, 0, screenWidth, screenHeight, 0, 0, screenWidth, screenHeight, GLBufferBit.Color, GLFunction.Nearest);

		GL.BindFramebuffer(GLBufferType.Framebuffer, 0);
		GL.ClearColor(ClearColor);
		GL.Clear(GLBufferBit.Color);
		GL.Disable(GLCapability.Depth);

		GL.BindVertexArray(_screenVertexArray);
		GL.ActiveTexture(0);
		GL.BindTexture(GLTextureType.Texture2D, _screenTexture);
		GL.DrawArrays(GLBeginMode.Triangles, 0, 6);

		PerformanceTrace.RunAndTrace(Window.SwapBuffers, "Window.SwapBuffers");

		if (AzaleaSettings.IgnoreGLError == false)
			PerformanceTrace.RunAndTrace(GL.PrintErrors, "GL.PrintErrors");
	}

	protected override void SetViewportImplementation(Vector2Int size)
	{
		GL.Viewport(0, 0, size.X, size.Y);
	}

	protected internal override void SetClearColor(Color value)
		=> GL.ClearColor(value);

	protected override void ClearImplementation(Color color)
		=> GL.Clear(GLBufferBit.Color);

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new GLVertexBatch<TexturedVertex2D>(Window, size);

	protected override INativeTexture CreateNativeTexture(int width, int height)
		=> new GLTexture(this, width, height);

	protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
	{
		if (texture is null)
		{
			GL.ActiveTexture((uint)unit);
			GL.BindTexture(GLTextureType.Texture2D, 0);
			return true;
		}

		switch (texture)
		{
			case GLTexture glTexture:
				glTexture.Bind((uint)unit);
				break;
		}

		return true;
	}

	#region Scissor test

	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle)
	{
		if (scissorRectangle.Width < 0) scissorRectangle.Width = 0;
		if (scissorRectangle.Height < 0) scissorRectangle.Height = 0;

		var framebufferHeight = Window.ClientSize.Y;

		GL.Scissor(scissorRectangle.X, framebufferHeight - scissorRectangle.Y - scissorRectangle.Height, scissorRectangle.Width, scissorRectangle.Height);
	}

	protected override void SetScissorTestState(bool enabled)
	{
		if (enabled)
			GL.Enable(GLCapability.ScissorTest);
		else
			GL.Disable(GLCapability.ScissorTest);
	}

	#endregion
}
