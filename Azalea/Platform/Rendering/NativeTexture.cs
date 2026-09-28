using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Azalea.Platform.Rendering;
public abstract class NativeTexture
{
	public int Width { get; private set; }
	public int Height { get; private set; }

	internal void SetSize(int width, int heigth)
	{
		Width = width;
		Height = heigth;
	}

	internal virtual bool IsReady() => true;

	private readonly object _lock = new();
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
