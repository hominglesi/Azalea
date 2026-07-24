using Azalea.Platform.Rendering.Coordination;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Threading;
using Azalea.Utils;
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

		Thread = new RenderThread(this);
	}

	/// <summary> Called after a rendering implementation has finished constructing.</summary>
	protected void StartRenderThread() => Thread.Start();

	protected abstract void Initialize();
	protected abstract void Update();

	private RenderCoordinator? _coordinator;
	public RenderCoordinator Coordinator
	{
		get => _coordinator ??= new RenderCoordinator(this);
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

	internal readonly ReadOnlyObservable<int> StagedQueueOverrides = new(0);
	internal readonly ReadOnlyObservable<int> NoStagedQueueFrames = new(0);

	internal void StageQueue(RenderCommandQueue queue)
	{
		lock (_stagedQueueLock)
		{
			if (_stagedQueue is not null)
			{
				_stagedQueue.Return();
				StagedQueueOverrides.Value++;
			}

			_stagedQueue = queue;
		}
	}

	internal bool SnapshotNextFrame = false;
	internal event Action<List<string>> CommandSnapshotCreated;

	protected void ProcessStagedQueue()
	{
		lock (_workingQueueLock)
		{
			lock (_stagedQueueLock)
			{
				if (_stagedQueue is null)
				{
					NoStagedQueueFrames.Value++;
					return;
				}

				_workingQueue = _stagedQueue;
				_stagedQueue = null;
			}

			lock (_commandsLock)
			{
				var commandSnapshot = new List<string>();
				var nextCommand = _workingQueue.Dequeue();

				while (nextCommand is not null)
				{
					commandSnapshot.Add(nextCommand.ToString()!);
					HandleCommand(nextCommand);
					nextCommand = _workingQueue.Dequeue();
				}

				_workingQueue.Return();

				if (SnapshotNextFrame)
				{
					CommandSnapshotCreated?.Invoke(commandSnapshot);
					SnapshotNextFrame = false;
				}
			}
		}
	}

	#endregion

	public readonly ReadOnlyObservable<bool> Stopped = new(false);
	internal void Stop()
	{
		if (Stopped) return;

		Thread.Stop();
		Stopped.Value = true;
	}

	#region RenderThread

	internal readonly RenderThread Thread;

	internal class RenderThread(PlatformRenderer renderer) : GameThread(1)
	{
		private readonly PlatformRenderer _renderer = renderer;

		public override string DisplayName => "Rendering Thread";

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

	public static PlatformRenderer AttachRenderer(Windowing.PlatformWindow window)
	{
		var deviceContext = window.BorrowDeviceContext();

		// For now OpenGL is hardcoded
		var renderer = new GLRenderer(deviceContext);

		window.Subscribe(renderer);
		return renderer;
	}
}
