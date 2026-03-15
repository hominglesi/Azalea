using Azalea.Platform.Windows;
using System.Threading;

namespace Azalea.Threading;
internal abstract class GameThread
{
	private readonly int _targetInterval;
	private volatile bool _running;
	private readonly Thread _thread;

	private WindowsWaitableTimer? _timer;

	public GameThread(int targetInterval)
	{
		_targetInterval = targetInterval;

		_running = true;
		_thread = new Thread(threadLoop)
		{
			IsBackground = true,
		};

		_thread.Start();
	}

	public void Start()
	{
		if (_running)
			return;

		_running = true;
		_thread.Start();
	}

	public void Stop()
	{
		if (_running == false)
			return;

		_running = false;
		_thread.Join();
	}

	private void threadLoop()
	{
		Thread.BeginThreadAffinity();

		_timer = new WindowsWaitableTimer(_targetInterval);
		_timer.Start();

		try
		{
			while (_running)
			{
				Work();

				_timer.Wait();
			}
		}
		finally
		{
			Thread.EndThreadAffinity();
			_timer.Dispose();
		}
	}

	protected abstract void Work();
}
