using Azalea.Utils;
using System;

namespace Azalea.Platform.Windowing;
public partial interface IPlatformDeviceContext
{
	public Vector2Int ClientSize { get; }
	event Action<Vector2Int> OnClientSizeChanged;
}
