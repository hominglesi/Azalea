using System;

namespace Azalea.Platform;
internal interface IAudioDeviceNotificationClient
{
	public event Action? DefaultDeviceChanged;
}
