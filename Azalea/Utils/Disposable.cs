using System;

namespace Azalea.Utils;
public abstract class Disposable : IDisposable
{
	protected readonly object DisposeLock = new();
	protected bool Disposed { get; private set; }

	public void Dispose()
	{
		dispose(true);
		GC.SuppressFinalize(this);
	}

	private void dispose(bool manualDispose)
	{
		lock (DisposeLock)
		{
			if (Disposed) return;

			OnDispose();
			Disposed = true;

			if (manualDispose == false)
				Console.WriteLine($"The object '{this}' was garbage collected before being manualy disposed.");
		}
	}

	protected abstract void OnDispose();

	~Disposable() => dispose(false);
}
