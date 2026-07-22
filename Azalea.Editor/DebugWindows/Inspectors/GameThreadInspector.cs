using Azalea.Editor.Design.Gui;
using Azalea.Threading;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class GameThreadInspector
{
	public static void Inject(GUIWindow window, GameThread thread)
	{
		window.AddGroup("Game Thread");

		var nativeThread = thread.NativeThread;
		window.AddGroup("Native Thread");
		window.AddLabel("Is Background: " + nativeThread.IsBackground);
		window.AddLabel("Managed Thread Id: " + nativeThread.ManagedThreadId);
		window.FinishGroup();

		var runningLabel = window.AddLabel("Running: " + thread.Running);
		thread.Running.OnValueChanged += running =>
			Scheduler.Schedule(() => runningLabel.Text = "Running: " + running);
		window.AddLabel("Display Name: " + thread.DisplayName);
		window.AddLabel($"Target Interval: {thread.TargetInterval}ms");
		window.AddLabel($"Target Frequency: {thread.TargetFrequency}ms");
		var avgIntervalLabel = window.AddLabel($"Average Interval: {thread.AverageInterval:0.00}ms");
		thread.AverageInterval.OnValueChanged += interval =>
			Scheduler.Schedule(() => avgIntervalLabel.Text = $"Average Interval: {interval:0.00}ms");
		var avgFrequencyLabel = window.AddLabel($"Average Frequency: {thread.AverageFrequency:0}hz");
		thread.AverageFrequency.OnValueChanged += frequency =>
			Scheduler.Schedule(() => avgFrequencyLabel.Text = $"Average Frequency: {frequency:0}hz");
		var avgWorkDurationLabel = window.AddLabel($"Average Work Duration: {thread.AverageWorkDuration:0.00}ms");
		thread.AverageWorkDuration.OnValueChanged += workDuration =>
			Scheduler.Schedule(() => avgWorkDurationLabel.Text = $"Average Work Duration: {workDuration:0.00}ms");

		window.FinishGroup();
	}
}
