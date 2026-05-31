using Azalea.Platform.Rendering.OpenGL;
using Azalea.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Azalea.Platform.Rendering;
public abstract class PlatformRenderer : IRenderCommandConsumer
{
	protected PlatformRenderer()
	{
		_commands = Channel.CreateUnbounded<RenderCommand>(new()
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

	private readonly Channel<RenderCommand> _commands;
	private readonly object _commandsLock = new();

	internal void Enqueue(RenderCommand command)
	{
		lock (_commandsLock)
		{
			if (_commands.Writer.TryWrite(command) == false)
				throw new Exception("Could not write command");
		}
	}
	void IRenderCommandConsumer.Enqueue(RenderCommand command) => Enqueue(command);

	private bool _commandGroupBegun = false;
	private List<RenderCommand> _commandGroup = [];
	public void BeginCommandGroup()
	{
		if (_commandGroupBegun)
			throw new Exception("Only one command group can be begun at a time");

		_commandGroupBegun = true;
	}

	public void SubmitCommandGroup()
	{
		if (_commandGroupBegun == false)
			throw new Exception("Command group has not been begun");

		lock (_commandsLock)
		{
			foreach (var command in _commandGroup)
				Enqueue(command);
		}

		_commandGroup.Clear();
		_commandGroupBegun = false;
	}

	internal abstract void HandleCommand(RenderCommand command);

	protected void HandleCommands()
	{
		lock (_commandsLock)
		{
			while (_commands.Reader.TryRead(out var command))
				HandleCommand(command);
		}
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
			_renderer.HandleCommands();
			_renderer.Update();
			_renderer.ProcessStagedQueue();
		}
	}

	#endregion
}
