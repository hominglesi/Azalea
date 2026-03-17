using System.Threading;

namespace Azalea.Threading;
internal class AtomicCounter
{
	private int _value;

	public void Increment() => Interlocked.Increment(ref _value);
	public void Decrement() => Interlocked.Decrement(ref _value);

	public bool IsActive => Volatile.Read(ref _value) > 0;
}
