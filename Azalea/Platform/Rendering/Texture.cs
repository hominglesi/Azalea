using System;
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
		_initializedEvent.Set();
	}

	private ManualResetEvent _initializedEvent = new(false);
	[MemberNotNull(nameof(NativeTexture))]
	internal void AssureInitialized()
	{
		if (NativeTexture is not null)
			return;

		var startTime = Time.GetCurrentPreciseTime();
		_initializedEvent.WaitOne();
		Console.WriteLine($"Waited for texture initialization {Time.GetPreciseMilisecondsSince(startTime)}ms");
	}
}

internal interface INativeTexture
{
	public uint Handle { get; }
}
