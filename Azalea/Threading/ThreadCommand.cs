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
public sealed class ThreadCommandAttribute(bool awaitable = false, bool generateHandler = true, string? displayName = null) : Attribute
{
	private bool _awaitable = awaitable;
	private bool _generateHandler = generateHandler;
	private string? _displayName = displayName;
}

public interface ICommandAwaitable
{
	public void Await();
}

public interface ICommandHandler<T>
	where T : ThreadCommand
{
	public ICommandAwaitable? Enqueue(T command);
}
