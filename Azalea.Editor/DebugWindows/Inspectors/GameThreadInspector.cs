using Azalea.Editor.Design.Gui;
using Azalea.Extentions;
using Azalea.Threading;
using Azalea.Utils;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class GameThreadInspector
{
	public static void Inject<T>(GUIWindow window, GameThread<T> thread)
		where T : ThreadCommand
	{
		window.AddGroup("Game Thread");

		var nativeThread = thread.NativeThread;
		window.AddGroup("Native Thread");
		window.AddLabel("Is Background: " + nativeThread.IsBackground);
		window.AddLabel("Managed Thread Id: " + nativeThread.ManagedThreadId);
		window.FinishGroup();

		window.AddPropertyLabel("Running", thread.CreateProxy<bool>("Running"));
		window.AddLabel("Display Name: " + thread.DisplayName);
		window.AddLabel($"Target Interval: {thread.TargetInterval}ms");
		window.AddLabel($"Target Frequency: {thread.TargetFrequency}ms");
		window.AddPropertyLabel("Average Interval", thread.CreateProxy<double>("AverageInterval"), "{0:0.00}ms");
		window.AddPropertyLabel("Average Frequency", thread.CreateProxy<double>("AverageFrequency"), "{0:0.00}ms");
		window.AddPropertyLabel("Average Work Duration", thread.CreateProxy<double>("AverageWorkDuration"), "{0:0.00}ms");

		window.FinishGroup();
	}
}
