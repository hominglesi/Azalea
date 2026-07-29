using System;
using System.Collections.Concurrent;

namespace Azalea.Utils;
public class ObjectPool<T>
	where T : IPoolable, new()
{
	private static ConcurrentBag<T>? _pool;

	public static T Borrow()
	{
		if (_pool is null || _pool.IsEmpty == false)
			return new T();

		if (_pool.TryTake(out T? existing))
			return existing;

		throw new Exception("Could not take item from pool!");
	}

	public static void Return(T poolable)
	{
		poolable.Reset();

		_pool ??= [];
		_pool.Add(poolable);
	}
}

public interface IPoolable
{
	public void Reset();
}
