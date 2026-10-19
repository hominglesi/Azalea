using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Azalea.Platform.Rendering;

public class Program : IShader
{
	internal uint? Handle { get; private set; }
	internal Dictionary<string, int>? UniformLocations;

	public Program() { }

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

		var startTime = GameHost.Instance.Loader.Thread.GetRunningTime();
		_initializedEvent.WaitOne();
		Console.WriteLine($"Waited for shader {GameHost.Instance.Loader.Thread.GetMilisecondsSince(startTime)}ms");
		Debug.Assert(Handle is not null);
	}
}

public interface IShader;
