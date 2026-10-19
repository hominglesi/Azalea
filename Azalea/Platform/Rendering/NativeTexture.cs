using System;
using System.Diagnostics;
using System.Threading;

namespace Azalea.Platform.Rendering;

public abstract class NativeTexture
{
	public int Width { get; internal set; }
	public int Height { get; internal set; }

	private readonly Lock _lock = new();
	public TextureState State { get; private set; } = TextureState.Loading;
	internal void SetState(TextureState state)
	{
		lock (_lock)
		{
			if (State == state) return;

			State = state;

			if (State == TextureState.Ready)
				_readyEvent.Set();
		}
	}

	protected virtual bool IsReady() => true;

	private readonly ManualResetEvent _readyEvent = new(false);
	internal void AssureReady()
	{
		lock (_lock)
		{
			if (State == TextureState.Ready)
			{
				Debug.Assert(IsReady());
				return;
			}

			var startTime = Time.GetCurrentPreciseTime();
			_readyEvent.WaitOne();
			Console.WriteLine($"Waited for texture {Time.GetPreciseMilisecondsSince(startTime)}ms");

			Debug.Assert(State == TextureState.Ready);
			Debug.Assert(IsReady());
		}
	}
}

public enum TextureState
{
	Ready = 0,
	Loading = 1,
	Missing = 2,
}
