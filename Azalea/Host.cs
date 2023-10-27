using Azalea.Platform;
using Azalea.Platform.Desktop;

namespace Azalea;

public static class Host
{
	public static GameHost CreateHost(HostPreferences preferences = default)
	{
		return new DesktopGameHost(preferences);
		/*
		return preferences.Type switch
		{
			HostType.Silk => new SilkGameHost(preferences),
			HostType.Veldrid => new VeldridGameHost(preferences),
			_ => throw new Exception($"Host of type {preferences.Type} does not exist")
		};*/
	}
}


