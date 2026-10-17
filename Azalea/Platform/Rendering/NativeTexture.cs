using System;
using System.Diagnostics;
using System.Threading;

namespace Azalea.Platform.Rendering;

public abstract class NativeTexture
{
	public int Width { get; internal set; }
	public int Height { get; internal set; }

	internal virtual bool IsReady() => true;

	private readonly Lock _lock = new();
	private volatile int _loadingOperations = 1;
	private readonly ManualResetEvent _readyEvent = new(false);
	internal void AssureReady()
	{
		lock (_lock)
		{
			if (_loadingOperations == 0)
			{
				Debug.Assert(IsReady());
				return;
			}

			var startTime = Time.GetCurrentPreciseTime();
			_readyEvent.WaitOne();
			Console.WriteLine($"Waited for texture {Time.GetPreciseMilisecondsSince(startTime)}ms");

			Debug.Assert(_loadingOperations == 0);
			Debug.Assert(IsReady());
		}
	}

	internal void BeginLoadingOperation()
	{
		lock (_lock)
		{
			if (_loadingOperations == 0)
				_readyEvent.Reset();

			_loadingOperations++;
		}
	}

	internal void FinishLoadingOperation()
	{
		_loadingOperations--;

		if (_loadingOperations == 0)
			_readyEvent.Set();
	}
}
