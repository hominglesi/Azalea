using Azalea.Inputs;
using System;

namespace Azalea.Platform.Windowing;
public class TrayIcon
{
	internal uint? Handle { get; private set; }

	internal TrayIcon() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("TrayIcon cannot be initialized multiple times!");

		Handle = handle;
	}

	public Action<MouseButton>? OnClick { get; set; }
	public Action? OnDoubleClick { get; set; }
}
