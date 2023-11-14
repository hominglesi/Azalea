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
		else
		{
			Console.WriteLine($"The object '{this}' was garbage collected before being properly disposed.");
		}

		Disposed = true;
	}

	protected abstract void OnDispose();

	~Disposable() => dispose(false);
}
