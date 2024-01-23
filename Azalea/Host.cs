using Azalea.Platform;

namespace Azalea;

public static class Host
{
	public static GameHost CreateHost(HostPreferences preferences = default)
	{
		return new DesktopGameHost(preferences);
	}
}


