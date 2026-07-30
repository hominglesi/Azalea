using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Azalea.Platform.Rendering;
public class Program
{
	internal uint? Handle { get; private set; }

	internal Program() { }

	private readonly ManualResetEvent _initializedEvent = new(false);
	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("Program cannot be initialized multiple times!");

		Handle = handle;
		_initializedEvent.Set();
	}

	[MemberNotNull(nameof(Handle))]
	internal void AssureInitialized()
	{
		if (Handle is not null)
			return;

		var startTime = Time.GetCurrentPreciseTime();
		_initializedEvent.WaitOne();
		Console.WriteLine($"Waited for shader {Time.GetPreciseMilisecondsSince(startTime)}ms");
		Debug.Assert(Handle is not null);
	}
}
