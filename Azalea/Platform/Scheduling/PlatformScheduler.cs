using Azalea.Platform.Rendering;
using Azalea.Threading;
using System;
using System.Diagnostics;

namespace Azalea.Platform.Scheduling;
public class PlatformScheduler
{
	private Windowing.PlatformWindow _window;

	protected PlatformScheduler(Windowing.PlatformWindow window)
	{
		_window = window;

		_thread = new ScheduleThread(this);
		_thread.Start();
	}

	public void Close()
	{
		_thread.Stop();
	}

	public static PlatformScheduler AttachScheduler(Windowing.PlatformWindow window)
	{
		var scheduler = new PlatformScheduler(window);

		window.Subscribe(scheduler);
		return scheduler;
	}

	public void InjectProtocol(Action<Windowing.PlatformWindow, PlatformRenderer> protocol)
		=> _thread.InjectProtocol(protocol);

	#region Thread

	private readonly ScheduleThread _thread;

	class ScheduleThread(PlatformScheduler scheduler) : GameThread(1)
	{
		public override string DisplayName => "Schedule Thread";

		private readonly PlatformScheduler _scheduler = scheduler;

		private PlatformRenderer? _renderer;

		protected override void Initialize()
		{
			if (_scheduler._window.SubscribedRenderer is null)
				throw new Exception("For now a window has to have a renderer to enable scheduling!");

			_renderer = _scheduler._window.SubscribedRenderer;
		}

		protected override void Update()
		{
			Debug.Assert(_renderer is not null);
			var protocol = _protocol;

			protocol?.Invoke(_scheduler._window, _renderer);
		}

		private readonly object _protocolInjectionLock = new();
		private Action<Windowing.PlatformWindow, PlatformRenderer>? _protocol = null;

		public void InjectProtocol(Action<Windowing.PlatformWindow, PlatformRenderer> protocol)
		{
			lock (_protocolInjectionLock)
			{
				if (_protocol is not null)
					throw new Exception("Only one protocol can be injected!");

				_protocol = protocol;
			}
		}
	}

	#endregion
}
