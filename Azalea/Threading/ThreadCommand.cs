using System;

namespace Azalea.Threading;
public abstract class ThreadCommand
{
	public static volatile int TotalCreated = 0;
	internal static Action<ThreadCommand>? OnCommandCreated;

	internal ThreadCommand()
	{
		TotalCreated++;
	}

	public abstract void Return();
	protected virtual void Cleanup() { }

	public override string ToString() => GetType().Name;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ThreadCommandAttribute(bool awaitable = false) : Attribute
{
	private bool _awaitable = awaitable;
}

public interface ICommandAwaitable
{
	public void Await();
}
