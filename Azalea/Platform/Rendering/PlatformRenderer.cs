using Azalea.Graphics.Colors;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Threading;
using System;
using System.Threading.Channels;

namespace Azalea.Platform.Rendering;
public abstract class PlatformRenderer
{
	protected PlatformRenderer()
	{
		_priorityCommands = Channel.CreateUnbounded<RenderCommand>(new()
		{
			SingleReader = true
		});

		_thread = new RenderThread(this);
		_thread.Start();
	}

	protected abstract void Initialize();
	protected abstract void Update();

	public static PlatformRenderer AttachRenderer(Windowing.PlatformWindow window)
	{
		var deviceContext = window.BorrowDeviceContext();

		// For now OpenGL is hardcoded
		var renderer = new GLRenderer(deviceContext);
		return renderer;
	}

	public abstract void Clear(Color color);

	#region Commands

	private readonly Channel<RenderCommand> _priorityCommands;

	internal void IssuePriorityCommand(RenderCommand command)
	{
		if (_priorityCommands.Writer.TryWrite(command) == false)
			Console.WriteLine("Could not write command");
	}

	internal abstract void HandleCommand(RenderCommand command);

	protected void HandlePriorityCommands()
	{
		while (_priorityCommands.Reader.TryRead(out var command))
			HandleCommand(command);
	}

	#endregion

	#region Framebuffer

	public abstract Framebuffer CreateFramebuffer();

	protected Framebuffer? BoundFramebuffer { get; private set; } = null;
	protected abstract void BindFramebufferImplementation(Framebuffer? framebuffer);
	public void BindFramebuffer(Framebuffer? framebuffer)
	{
		if (framebuffer == BoundFramebuffer)
			return;

		BindFramebufferImplementation(framebuffer);
		BoundFramebuffer = framebuffer;
	}

	#endregion

	#region Texture2D

	public abstract Texture2D CreateTexture2D();

	protected Texture2D? BoundTexture2D { get; private set; } = null;
	public abstract void BindTexture2DImplementation(Texture2D texture2D);
	public void BindTexture2D(Texture2D texture2D)
	{
		if (texture2D == BoundTexture2D)
			return;

		BindTexture2DImplementation(texture2D);
		BoundTexture2D = texture2D;
	}

	#endregion

	#region Thread

	private readonly RenderThread _thread;

	class RenderThread(PlatformRenderer renderer) : GameThread(1)
	{
		public override string DisplayName => "Rendering Thread";

		private readonly PlatformRenderer _renderer = renderer;

		protected override void Initialize()
		{
			_renderer.Initialize();
		}

		protected override void Update()
		{
			_renderer.HandlePriorityCommands();
			_renderer.Update();
		}
	}

	#endregion
}
