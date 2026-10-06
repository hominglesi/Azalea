using Azalea.Graphics;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Platform.Windows;
using System;

namespace Azalea.Platform;

internal class DesktopGameHost : GameHost
{
	internal DesktopGameHost(HostPreferences prefs)
		: base(prefs)
	{
		if (prefs.PersistentDirectory is not null)
			Assets.SetupPersistentStore(prefs.PersistentDirectory);

		if (prefs.ReflectedDirectory is not null)
			Assets.SetupReflectedStore(prefs.ReflectedDirectory);

		if (prefs.ConfigName is not null)
			ConfigProvider = new FileConfigProvider(prefs.ConfigName);
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);

		ConfigProvider?.Save();
	}

	protected override void RunGameLoop()
	{
		ProcessGameLoop();
	}

	internal override IClipboard CreateClipboard()
		=> new WindowsClipboard();

	public override ITrayIcon CreateTrayIcon(string iconName, Image icon)
	{
		ArgumentNullException.ThrowIfNull(icon, nameof(icon));

		return null;
	}
}
