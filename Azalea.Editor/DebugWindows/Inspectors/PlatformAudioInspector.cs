using Azalea.Design.Containers;
using Azalea.Editor.Design.Gui;
using Azalea.Extentions.ObjectExtentions;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform.Audio;
using Azalea.Platform.Audio.OpenAL;
using Azalea.Threading;
using Azalea.Utils;
using System;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class PlatformAudioInspector
{
	public static GUIWindow Create(PlatformAudio audio, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create("PlatformAudio", position, new(400, 400));

		var loopingCheckbox = window.AddCheckbox("Looping", false);
		var gainSlider = window.AddSliderFloat("Gain", 0, 1, 0.2f, "0.00");

		window.AddButton("Play click", () => audio.PlayByte(Assets.MainStore.GetSoundByteNew("Audio/click.wav"), gainSlider.Slider.Value, loopingCheckbox.Checked));

		window.AddButton("Play temp", () => audio.PlayByte(Assets.MainStore.GetSoundByteNew("Audio/goodkidByte.wav"), gainSlider.Slider.Value, loopingCheckbox.Checked));

		if (audio is ALAudio alAudio)
		{
			window.AddGroup($"ByteSources (Count: {alAudio.AudioByteSources.Length})");

			for (int i = 0; i < alAudio.AudioByteSources.Length; i++)
			{
				var index = i;
				var byteSourceGroup = window.AddGroup($"ByteSource {i}");

				window.FinishGroup();

				alAudio.AudioByteSources[i].OnCurrentInstanceChanged +=
					instance => Scheduler.Schedule(() =>
					{
						if (instance is not null)
						{
							byteSourceGroup.TitleBar.Label.Text = $"ByteSource {index} (Playing)";
							window.SelectGroup(byteSourceGroup);
							window.Add(new AudioInstanceInspector(window, instance));
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

			var instanceInspector = new AudioInstanceInspector(window, instance);
			window.Add(instanceInspector);

			window.FinishGroup();
		});

		return window;
	}

	class AudioInstanceInspector : FlexContainer
	{
		private readonly ObservableProxy<AudioInstanceState> _instanceState;

		public AudioInstanceInspector(GUIWindow window, IAudioInstance instance)
		{
			_instanceState = instance.CreateProxy<AudioInstanceState>("State");

			RelativeSizeAxes = Axes.X;
			AutoSizeAxes = Axes.Y;
			Direction = FlexDirection.Vertical;
			Spacing = new(0, 4);

			window.SelectGroup(this);

			if (instance is AudioByteInstance)
			{
				var durationText = TextUtils.FormatTimeCodeFromSeconds(instance.Duration);

				window.AddLabel($"Looping: {instance.Looping}");
				window.AddLabel($"Duration: {durationText}");
				window.AddObservingLabel("State", instance.CreateProxy<AudioInstanceState>("State"));

				var gainSlider = window.AddSliderFloat("Gain", 0, 1, instance.Gain, "0.00");
				gainSlider.OnValueChanged(instance.SetGain);

				var timestampSlider = window.AddObservingSliderFloat("Timestamp",
					instance.CreateProxy<float>("Timestamp", instance.SetTimestamp),
					0, (float)instance.Duration, "0.00");

				var buttonGroup = new FlexContainer()
				{
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y,
					Spacing = new(4, 0)
				};
				window.Add(buttonGroup);
				window.SelectGroup(buttonGroup);

				window.AddButton("Pause", instance.Pause);
				window.AddButton("Unpause", instance.Unpause);
				window.AddButton("Stop", instance.Stop);

				window.FinishGroup();
			}

			window.FinishGroup();
		}

		protected override void Update()
		{
			if (_instanceState.TryGetInvalid(out AudioInstanceState newState))
				if (newState == AudioInstanceState.Stopped)
					Parent!.Remove(this);
		}
	}
}
