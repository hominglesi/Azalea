using System;

namespace Azalea.Platform.Audio;
internal interface IAudioDeviceNotificationClient
{
	public event Action? DefaultDeviceChanged;
}
