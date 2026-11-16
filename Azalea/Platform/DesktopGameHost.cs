using Azalea.IO.Configs;
using Azalea.Platform.Audio;
using Azalea.Platform.Audio.OpenAL;
using Azalea.Platform.Audio.Windows;
using Azalea.Platform.Loading;
using Azalea.Platform.Loading.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using Azalea.Platform.Windows;

namespace Azalea.Platform;

internal class DesktopGameHost : GameHost
{
	protected override PlatformLoader CreateLoader() => new GLLoader();
	protected override PlatformAudio CreateAudio() => new ALAudio(new WindowsAudioDeviceNotificationClient());
	protected override IClipboard CreateClipboard() => new WindowsClipboard();
	protected override IConfigProvider? CreateConfigProvider(string path) => new FileConfigProvider(path);

	internal override PlatformWindow CreateWindow(Vector2Int size, bool initiallyVisible = true, bool initializeOle = true)
		=> new WindowsWindow(size, initiallyVisible, initializeOle);
	internal override PlatformRenderer CreateRenderer(IPlatformDeviceContext deviceContext) => new GLRenderer(deviceContext);
}
