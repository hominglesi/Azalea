using Azalea.Threading;
using System;
using System.Threading;
using System.Threading.Channels;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;

/// <summary>
/// An OpenGL context used for loading assets which all other contexts
/// use as a shared context.
/// </summary>
internal partial class GLLoadingContext
{
	public readonly nint Handle;

	public GLLoadingContext()
	{
		Thread = new GLLoadingThread();
		Thread.Start();

		Thread.Initialized.WaitOne();
		Thread.Initialized.Dispose();

		Handle = Thread.Context!.Handle;
	}

	#region LoadingThread

	internal readonly GLLoadingThread Thread;

	internal partial class GLLoadingThread() : GameThread(1)
	{
		private Windowing.PlatformWindow? _window;
		internal GLContext? Context;

		public override string DisplayName => "GL Loading Thread";
		internal EventWaitHandle Initialized = new(false, EventResetMode.ManualReset);

		protected override void Initialize()
		{
			_window = Windowing.PlatformWindow.Create(initiallyVisible: false);
			Context = GLContext.Create(_window.BorrowDeviceContext());
			Context.MakeCurrent();

			Initialized.Set();
		}

		private readonly Channel<LoadingCommand> _commands = Channel.CreateUnbounded<LoadingCommand>(new()
		{
			SingleReader = true
		});
		private readonly object _commandsLock = new();
		internal ICommandAwaitable? Enqueue(LoadingCommand command)
		{
			lock (_commandsLock)
			{
				if (_commands.Writer.TryWrite(command) == false)
					throw new Exception("Could not write command");

				if (command is ICommandAwaitable awaitable)
					return awaitable;

				return null;
			}
		}

		protected override void Update()
		{
			lock (_commandsLock)
			{
				while (_commands.Reader.TryRead(out var command))
					handleCommand(command);
			}
		}
	}

	#endregion
}
