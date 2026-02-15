using System.Collections.Generic;

namespace Azalea.Threading;
internal static class PromiseSystem
{
	private static readonly List<IPromise> _promises = [];

	public static void AddPromise(IPromise promise)
	{
		lock (_promises)
			_promises.Add(promise);
	}

	public static void ProcessPromises()
	{
		lock (_promises)
		{
			for (int i = 0; i < _promises.Count; i++)
			{
				var promise = _promises[i];

				if (promise.IsResolvedInternal)
				{
					promise.ExecuteOnResolved();

					_promises.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
