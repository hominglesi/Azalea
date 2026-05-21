using System;

namespace Azalea.Platform.Windowing.Windows;
internal class WindowsWindow : PlatformWindow
{
	public override string PlatformType => "Windows";

	protected override void Initialize()
	{
		var processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		Console.WriteLine($"Initialized from thread {Environment.CurrentManagedThreadId}");
	}

	protected override void Update()
	{
		Console.WriteLine($"Windows updated from thread {Environment.CurrentManagedThreadId}");
	}
}
