using Azalea.Platform.Rendering;
using Azalea.Threading;
using System;
using System.Diagnostics;

namespace Azalea.Platform.Scheduling;
public class PlatformScheduler
{
	protected PlatformScheduler(Windowing.PlatformWindow window)
	{
		Thread = new ScheduleThread(window, 1);
		Thread.Start();
	}

	public bool Stopped => !Thread.Running;
	public void Stop()
	{
		if (Thread.Running == false) return;

		Thread.Stop();
	}

	public void InjectProtocol(Action<Windowing.PlatformWindow, PlatformRenderer> protocol)
		=> Thread.InjectProtocol(protocol);

	#region ScheduleThread

	internal readonly ScheduleThread Thread;

	internal class ScheduleThread : GameThread
	{
		internal readonly Windowing.PlatformWindow Window;
		internal readonly PlatformRenderer Renderer;

		public override string DisplayName => "Schedule Thread";

		public ScheduleThread(Windowing.PlatformWindow window, int interval)
			: base(interval)
		{
			Window = window;
			//For now a window has to have a renderer to enable scheduling!
			Debug.Assert(window.SubscribedRenderer is not null);
			Renderer = window.SubscribedRenderer;
		}

		protected override void Initialize() { }
		protected override void Update() => Protocol?.Invoke(Window, Renderer);

		private readonly object _protocolInjectionLock = new();
		internal Action<Windowing.PlatformWindow, PlatformRenderer>? Protocol = null;

		public void InjectProtocol(Action<Windowing.PlatformWindow, PlatformRenderer> protocol)
		{
			lock (_protocolInjectionLock)
			{
				if (Protocol is not null)
					throw new Exception("Only one protocol can be injected!");

				Protocol = protocol;
			}
		}
	}

	#endregion

	public static PlatformScheduler AttachScheduler(Windowing.PlatformWindow window)
	{
		var scheduler = new PlatformScheduler(window);

		window.Subscribe(scheduler);
		return scheduler;
	}
}
