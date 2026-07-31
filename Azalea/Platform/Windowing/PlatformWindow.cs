using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Platform.Windowing.Windows;
using Azalea.Threading;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azalea.Platform.Windowing;
public abstract class PlatformWindow
{
	protected PlatformWindow(Vector2Int clientSize, bool initiallyVisible)
	{
		Shown = new(initiallyVisible);
		ClientSize = new(clientSize);

		Thread = new WindowThread(this);
		Thread.Start();

		Thread.Initialized.WaitOne();
		Thread.Initialized.Dispose();
	}

	public static PlatformWindow Create(Vector2Int size, bool initiallyVisible = true)
	{
		var newWindow = RuntimeInformation.ProcessArchitecture switch
		{
			Architecture.X64 or Architecture.X86 => new WindowsWindow(size, initiallyVisible),
			_ => throw new NotSupportedException(
				$"Platform '{RuntimeInformation.ProcessArchitecture}' is not supported")
		};

		return newWindow;
	}

	public virtual string PlatformType => "Abstract Window";
	public string Title => "Azalea Window";
	public readonly ReadOnlyObservable<bool> Shown;

	public ReadOnlyObservable<Vector2Int> ClientSize { get; }

	protected abstract void Initialize();
	protected abstract void Update();

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

	public readonly ReadOnlyObservable<bool> Closed = new(false);
	public void Close()
	{
		if (Closed) return;

		Thread.Stop();
		SubscribedRenderer?.Stop();
		SubscribedScheduler?.Stop();

		Closed.Value = true;
	}

	#region WindowThread

	internal readonly WindowThread Thread;

	internal class WindowThread(PlatformWindow window) : GameThread(1)
	{
		private readonly PlatformWindow _window = window;

		public override string DisplayName => "Window Thread";

		internal EventWaitHandle Initialized = new(false, EventResetMode.ManualReset);

		protected override void Initialize()
		{
			_window.Initialize();
			Initialized.Set();
		}

		protected override void Update()
		{
			_window.Update();
		}
	}

	#endregion
}
