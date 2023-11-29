using Azalea.Platform;
using Azalea.Platform.Desktop;

namespace Azalea;

public static class Host
{
	public static GameHost CreateHost(HostPreferences preferences = default)
	{
		return new DesktopGameHost(preferences);
	}
}


