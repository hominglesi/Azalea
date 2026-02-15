namespace Azalea.Threading;
internal interface IPromise
{
	bool IsResolvedInternal { get; }

	void ExecuteOnResolved();
}
