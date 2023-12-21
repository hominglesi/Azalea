using System;

namespace Azalea.Tests.Dummy;
public class DummyGame : AzaleaGame
{
	private Action? _initializeAction;
	public DummyGame(Action? initializeAction = null)
	{
		_initializeAction = initializeAction;
	}

	protected override void OnInitialize()
	{
		_initializeAction?.Invoke();
	}
}
