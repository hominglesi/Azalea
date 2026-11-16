using Azalea.Platform.Rendering;
using Azalea.Platform.Windowing;
using Azalea.Threading;
using System;
using System.Diagnostics;
using System.Threading;

namespace Azalea.Platform.Scheduling;

public class PlatformScheduler : ICommandHandler<ThreadCommand>
{
	internal PlatformScheduler(PlatformWindow window)
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

	public ICommandAwaitable? Enqueue(ThreadCommand command) => Thread.Enqueue(command);

	public void InjectProtocol(Action<PlatformWindow, PlatformRenderer> protocol)
		=> Thread.InjectProtocol(protocol);

	public DateTime GetRunningTime() => Thread.GetRunningTime();
	public float GetMilisecondsSince(DateTime time) => Thread.GetMilisecondsSince(time);
	public float DeltaTime => Thread.DeltaTime;

	#region ScheduleThread

	internal readonly ScheduleThread Thread;

	internal class ScheduleThread : GameThread
	{
		internal readonly PlatformWindow Window;
		internal readonly PlatformRenderer Renderer;

		public override string DisplayName => "Schedule Thread";

		public ScheduleThread(PlatformWindow window, int interval)
			: base(interval)
		{
			Window = window;

			Debug.Assert(window.SubscribedRenderer is not null);
			Renderer = window.SubscribedRenderer;
		}

		protected override void Initialize() { }
		protected override void Update() => Protocol?.Invoke(Window, Renderer);

		protected override void HandleCommand(ThreadCommand command)
		{
			switch (command)
			{
				case InvokeActionCommand(var action):
					action.Invoke();
					break;
				default:
					throw new NotImplementedException("Command handling hasn't been implemented");
			}

			command.Return();
		}

		private readonly Lock _protocolInjectionLock = new();
		internal Action<PlatformWindow, PlatformRenderer>? Protocol = null;

		public void InjectProtocol(Action<PlatformWindow, PlatformRenderer> protocol)
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
}
