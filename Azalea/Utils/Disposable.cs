using System;

namespace Azalea.Utils;
public abstract class Disposable : IDisposable
{
	protected bool Disposed { get; private set; }

	public void Dispose()
	{
		dispose(true);
		GC.SuppressFinalize(this);
	}

	private void dispose(bool disposing)
	{
		if (Disposed) return;

		if (disposing)
		{
			OnDispose();
		}

		Disposed = true;
	}

	protected abstract void OnDispose();

	~Disposable() => dispose(false);
}
