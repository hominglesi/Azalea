using Azalea.Editor.Design.Gui;
using Azalea.IO.Resources;
using Azalea.Platform.Audio;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformAudioInspector
{
	public static GUIWindow Create(PlatformAudio audio, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create("PlatformAudio", position, new(400, 400));

		window.AddButton("Play click", () => audio.PlayByte(Assets.MainStore.GetSoundByteNew("Audio/click.wav"), 0.2f, false));

		return window;
	}
}
