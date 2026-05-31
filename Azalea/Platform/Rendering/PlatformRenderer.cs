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

	private RenderCommandQueue? _stagedQueue = null;
	private readonly object _stagedQueueLock = new();
	private RenderCommandQueue? _workingQueue = null;
	private readonly object _workingQueueLock = new();

	internal void StageQueue(RenderCommandQueue queue)
	{
		lock (_stagedQueueLock)
			_stagedQueue = queue;
	}

	protected void ProcessStagedQueue()
	{
		lock (_workingQueueLock)
		{
			lock (_stagedQueueLock)
			{
				if (_workingQueue == _stagedQueue)
					return;

				if (_workingQueue is not null)
					RenderCommandQueue.Return(_workingQueue);

				_workingQueue = _stagedQueue;
			}

			if (_workingQueue is null)
				return;

			var nextCommand = _workingQueue.Dequeue();

			while (nextCommand is not null)
			{
				HandleCommand(nextCommand);
				nextCommand = _workingQueue.Dequeue();
			}
		}
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
			_renderer.ProcessStagedQueue();
		}
	}

	#endregion
}
