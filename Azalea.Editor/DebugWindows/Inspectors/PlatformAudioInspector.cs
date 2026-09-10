using Azalea.Editor.Design.Gui;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform.Audio;
using Azalea.Platform.Audio.OpenAL;
using Azalea.Threading;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformAudioInspector
{
	public static GUIWindow Create(PlatformAudio audio, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create("PlatformAudio", position, new(400, 400));

		window.AddButton("Play click", () => audio.PlayByte(Assets.MainStore.GetSoundByteNew("Audio/click.wav"), 0.2f, false));

		window.AddButton("Play temp", () => audio.PlayByte(Assets.MainStore.GetSoundByteNew("Audio/goodkidByte.wav"), 0.2f, false));

		if (audio is ALAudio alAudio)
		{
			window.AddGroup($"ByteSources (Count: {alAudio.AudioByteSources.Length})");

			for (int i = 0; i < alAudio.AudioByteSources.Length; i++)
			{
				var index = i;
				var byteSourceGroup = window.AddGroup($"ByteSource {i}");

				window.FinishGroup();

				alAudio.AudioByteSources[i].CurrentInstance.OnValueChanged +=
					instance => Scheduler.Schedule(() =>
					{
						if (instance is not null)
						{
							byteSourceGroup.TitleBar.Label.Text = $"ByteSource {index} (Playing)";
							window.SelectGroup(byteSourceGroup);
							injectInstanceInspector(window, instance);
							window.FinishGroup();
							byteSourceGroup.SetExpanded(true);
						}
						else
						{
							byteSourceGroup.TitleBar.Label.Text = $"ByteSource {index}";
							byteSourceGroup.Clear();
							byteSourceGroup.SetExpanded(false);
						}

					});
			}

			window.FinishGroup();
		}

		var activeInstances = window.AddGroup("Active Instances");

		window.FinishGroup();

		audio.InstanceStarted += instance => Scheduler.Schedule(() =>
		{
			window.SelectGroup(activeInstances);

			var instanceInspector = injectInstanceInspector(window, instance);

			instance.Stopped += () => Scheduler.Schedule(() =>
			{
				instanceInspector.Parent!.Remove(instanceInspector);
			});

			window.FinishGroup();
		});

		return window;
	}

	private static GameObject injectInstanceInspector(GUIWindow window, IAudioInstance instance)
	{
		if (instance is AudioByteInstance)
		{
			var durationText = TextUtils.FormatTimeCodeFromSeconds(instance.Duration);

			return window.AddLabel($"AudioInstance(Duration: {durationText})");
		}
		else return new GameObject();
	}
}
