using Azalea.Platform.Rendering.Coordination;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Threading;
using Azalea.Utils;
using System;
using System.Collections.Generic;

namespace Azalea.Platform.Rendering;
public abstract partial class PlatformRenderer : IRenderCommandConsumer
{
	protected PlatformRenderer()
	{
		Thread = new RenderThread(this);
	}

	/// <summary> Called after a rendering implementation has finished constructing. </summary>
	protected void StartRenderThread() => Thread.Start();

	protected abstract void InitializationLogic();
	internal abstract void HandleCommandLogic(RenderCommand command);

	private RenderCoordinator? _coordinator;
	public RenderCoordinator Coordinator
	{
		get => _coordinator ??= new RenderCoordinator(this);
	}

	#region Commands

	void IRenderCommandConsumer.Enqueue(RenderCommand command, ICommandGroup? group)
		=> Thread.Enqueue(command, group);

	public ICommandGroup BeginGroup() => Thread.CreateCommandGroup();

	private RenderCommandQueue? _stagedQueue = null;
	private readonly object _stagedQueueLock = new();

	internal readonly ReadOnlyObservable<int> StagedQueueOverrides = new(0);

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

	internal RenderCommandQueue? RequestQueue()
	{
		lock (_stagedQueueLock)
		{
			var stagedQueue = _stagedQueue;
			_stagedQueue = null;
			return stagedQueue;
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

	internal class RenderThread(PlatformRenderer renderer) : GameThread<RenderCommand>(1)
	{
		private readonly PlatformRenderer _renderer = renderer;

		internal readonly ReadOnlyObservable<int> NoStagedQueueFrames = new(0);

		internal bool SnapshotNextFrame = false;
		internal event Action<List<string>>? CommandSnapshotCreated;

		public override string DisplayName => "Rendering Thread";

		protected override void Initialize() => _renderer.InitializationLogic();
		protected override void HandleCommand(RenderCommand command)
			=> _renderer.HandleCommandLogic(command);

		protected override void Update()
		{
			processStagedQueue();
		}

		private void processStagedQueue()
		{
			var stagedQueue = _renderer.RequestQueue();

			if (stagedQueue is null)
			{
				NoStagedQueueFrames.Value++;
				return;
			}

			var commandSnapshot = SnapshotNextFrame ? new List<string>() : null;

			var nextCommand = stagedQueue.Dequeue();
			while (nextCommand is not null)
			{
				commandSnapshot?.Add(nextCommand.ToString());
				HandleCommand(nextCommand);
				nextCommand = stagedQueue.Dequeue();
			}

			stagedQueue.Return();

			if (commandSnapshot is not null)
			{
				CommandSnapshotCreated?.Invoke(commandSnapshot);
				SnapshotNextFrame = false;
			}
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
