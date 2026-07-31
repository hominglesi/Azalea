using Azalea.Utils;

namespace Azalea.Platform.Windowing;
public interface IPlatformDeviceContext
{
	public ReadOnlyObservable<Vector2Int> ClientSize { get; }
}
