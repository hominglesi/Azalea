using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Azalea.Platform.Rendering;
public class Texture
{
	internal INativeTexture? NativeTexture { get; private set; }

	internal Texture() { }

	internal void Initialize(INativeTexture nativeTexture)
	{
		if (NativeTexture is not null)
			throw new Exception("Texture cannot be initialized multiple times!");

		NativeTexture = nativeTexture;
		FinishLoadingOperation();
	}

	private readonly object _lock = new();
	private volatile int _loadingOperations = 1;
	private readonly ManualResetEvent _readyEvent = new(false);
	[MemberNotNull(nameof(NativeTexture))]
	internal void AssureReady()
	{
		lock (_lock)
		{
			if (_loadingOperations == 0)
			{
				Debug.Assert(NativeTexture is not null);
				return;
			}

			var startTime = Time.GetCurrentPreciseTime();
			_readyEvent.WaitOne();
			Console.WriteLine($"Waited for texture {Time.GetPreciseMilisecondsSince(startTime)}ms");

			Debug.Assert(_loadingOperations == 0);
			Debug.Assert(NativeTexture is not null);
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

internal interface INativeTexture
{
	public uint Handle { get; }
}
