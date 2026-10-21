using Azalea.IO.Configs;
using Azalea.Platform.Audio;
using Azalea.Platform.Audio.OpenAL;
using Azalea.Platform.Audio.Windows;
using Azalea.Platform.Loading;
using Azalea.Platform.Loading.OpenGL;
using Azalea.Platform.Windows;

namespace Azalea.Platform;

internal class DesktopGameHost : GameHost
{
	protected override PlatformLoader CreateLoader() => new GLLoader();
	protected override PlatformAudio CreateAudio() => new ALAudio(new WindowsAudioDeviceNotificationClient());
	protected override IClipboard CreateClipboard() => new WindowsClipboard();
	protected override IConfigProvider? CreateConfigProvider(string path) => new FileConfigProvider(path);
}
