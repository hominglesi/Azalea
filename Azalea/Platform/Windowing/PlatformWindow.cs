using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Platform.Windowing.Windows;
using Azalea.Threading;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;

namespace Azalea.Platform.Windowing;
public abstract partial class PlatformWindow : ICommandHandler<WindowCommand>
{
	protected PlatformWindow(string title, Vector2Int clientSize, bool initiallyVisible)
	{
		Title = title;
		Shown = initiallyVisible;
		Position = new Vector2Int(100, 100);
		ClientPosition = new Vector2Int(100, 100);
		Size = clientSize;
		ClientSize = clientSize;
		Resizable = true;
		CursorVisible = true;

		Thread = new WindowThread(this);
		Thread.Start();

		Thread.Initialized.WaitOne();
		Thread.Initialized.Dispose();
	}

	protected abstract void InitializationLogic();
	protected abstract void UpdateLogic();
	protected abstract void HandleCommandLogic(WindowCommand command);

	public virtual string PlatformType => "Abstract Window";

	public string Title { get; protected set; }
	public bool Shown { get; protected set; }
	public Vector2Int Position { get; protected set; }
	public Vector2Int ClientPosition { get; protected set; }
	public Vector2Int Size { get; protected set; }
	public Vector2Int ClientSize
	{
		get;
		protected set { if (field == value) return; field = value; OnResized?.Invoke(value); }
	}
	internal event Action<Vector2Int>? OnResized;
	public bool Resizable { get; protected set; }
	public bool CursorVisible { get; protected set; }
	public bool Closed
	{
		get;
		set { if (field == value) return; field = value; OnClosed?.Invoke(); }
	}

	public event Action? OnClosed;

	public PlatformRenderer? SubscribedRenderer { get; private set; } = null;
	internal void Subscribe(PlatformRenderer renderer)
	{
		if (SubscribedRenderer is not null)
			throw new Exception("Only one rendered can be subscribed at a time");

		SubscribedRenderer = renderer;
	}

	public PlatformScheduler? SubscribedScheduler { get; private set; } = null;
	internal void Subscribe(PlatformScheduler scheduler)
	{
		if (SubscribedScheduler is not null)
			throw new Exception("Only one scheduler can be subscribed at a time");

		SubscribedScheduler = scheduler;
	}

	internal bool DeviceContextBorrowed = false;
	private readonly object _deviceContextOwnerLock = new();
	protected abstract IPlatformDeviceContext GetDeviceContext();
	public IPlatformDeviceContext BorrowDeviceContext()
	{
		lock (_deviceContextOwnerLock)
		{
			if (DeviceContextBorrowed)
				throw new InvalidOperationException("Device Context is already in use!");

			DeviceContextBorrowed = true;

			return GetDeviceContext();
		}
	}

	internal readonly Channel<InputEvent> PendingInputEvents = Channel.CreateUnbounded<InputEvent>(
		new() { SingleReader = true }
	);

	protected void EnqueueInputEvent(InputEvent e)
	{
		if (PendingInputEvents.Writer.TryWrite(e) == false)
			throw new Exception("Could not write command!");
	}

	public void Close()
	{
		if (Closed) return;

		Thread.Stop();
		SubscribedRenderer?.Stop();
		SubscribedScheduler?.Stop();

		Closed = true;
	}

	public ICommandAwaitable? Enqueue(WindowCommand command) => Thread.Enqueue(command);

	#region WindowThread

	internal readonly WindowThread Thread;

	internal class WindowThread(PlatformWindow window) : GameThread<WindowCommand>(1)
	{
		private readonly PlatformWindow _window = window;

		public override string DisplayName => "Window Thread";

		internal EventWaitHandle Initialized = new(false, EventResetMode.ManualReset);

		protected override void Initialize()
		{
			_window.InitializationLogic();
			Initialized.Set();
		}

		protected override void Update()
		{
			_window.UpdateLogic();
		}

		protected override void HandleCommand(WindowCommand command)
			=> _window.HandleCommandLogic(command);
	}

	#endregion

	public static PlatformWindow Create(string title, Vector2Int size, bool initiallyVisible = true)
	{
		var newWindow = RuntimeInformation.ProcessArchitecture switch
		{
			Architecture.X64 or Architecture.X86 => new WindowsWindow(title, size, initiallyVisible),
			_ => throw new NotSupportedException(
				$"Platform '{RuntimeInformation.ProcessArchitecture}' is not supported")
		};

		return newWindow;
	}
}
