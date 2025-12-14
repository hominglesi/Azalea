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

	private void dispose(bool manualDispose)
	{
		if (Disposed) return;

		OnDispose();
		Disposed = true;

		if (manualDispose == false)
			Console.WriteLine($"The object '{this}' was garbage collected before being manualy disposed.");
	}

	protected abstract void OnDispose();

	~Disposable() => dispose(false);
}
