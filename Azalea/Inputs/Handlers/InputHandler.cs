using Azalea.Platform;
using System;

namespace Azalea.Inputs.Handlers;

public abstract class InputHandler
{
	private bool isInitialized;

	public virtual bool Initialize(GameHost host)
	{
		if (isInitialized)
			throw new InvalidOperationException($"{nameof(Initialize)} was run more than once");

		isInitialized = true;
		return true;
	}
}
