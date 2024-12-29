namespace Azalea.Utils;
public abstract class UnmanagedObject<T> : Disposable
{
	public T Handle { get; init; }

	protected UnmanagedObject()
	{
		Handle = CreateObject();
	}

	protected abstract T CreateObject();
}
