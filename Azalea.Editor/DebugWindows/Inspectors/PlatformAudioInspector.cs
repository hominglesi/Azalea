using Azalea.Design.Containers;
using Azalea.Editor.Design.Gui;
using Azalea.Extentions;
using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Platform.Audio;
using Azalea.Platform.Audio.OpenAL;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.Editor.DebugWindows.Inspectors;

internal class PlatformAudioInspector
{
	public static GUIWindow Create(Application app, PlatformAudio audio, GUIWindow? origin = null)
	{
		var position = origin is null ? new(100, 100) : origin.Position + new Vector2(20, 20);
		var window = GUIWindow.Create(app, "PlatformAudio", position, new(400, 400));

		var loopingCheckbox = window.AddCheckbox("Looping", false);
		var gainSlider = window.AddSliderFloat("Gain", 0, 1, 0.2f, "0.00");

		window.AddButton("Play click", () => audio.PlayByte(Assets.MainStore.GetSoundByte("Audio/click.wav"), gainSlider.Slider.Value, loopingCheckbox.Checked));

		window.AddButton("Play temp", () => audio.PlayByte(Assets.MainStore.GetSoundByte("Audio/goodkidByte.wav"), gainSlider.Slider.Value, loopingCheckbox.Checked));
		window.AddButton("Play temp 2", () => audio.Play(Assets.MainStore.GetSound("Audio/deargod.mp3"), gainSlider.Slider.Value, loopingCheckbox.Checked));

		if (audio is ALAudio alAudio)
		{
			window.AddGroup($"AudioSources (Count: {alAudio.AudioSources.Length})");

			for (int i = 0; i < alAudio.AudioSources.Length; i++)
				window.Add(new AudioSourceInspector(window, i, alAudio.AudioSources[i]));

			window.FinishGroup();

			window.AddGroup($"ByteSources (Count: {alAudio.AudioByteSources.Length})");

			for (int i = 0; i < alAudio.AudioByteSources.Length; i++)
				window.Add(new ByteSourceInspector(window, i, alAudio.AudioByteSources[i]));

			window.FinishGroup();
		}

		return window;
	}

	class AudioSourceInspector : FlexContainer
	{
		private readonly GUIWindow _window;
		private readonly int _index;
		private readonly GUILabel _label;
		private IAudioInstance? _instance;
		private IAudioInstance? _newInstance;

		public AudioSourceInspector(GUIWindow window, int index, ALSource source)
		{
			_window = window;
			_index = index;

			Direction = FlexDirection.Vertical;
			RelativeSizeAxes = Axes.X;
			AutoSizeAxes = Axes.Y;

			window.SelectGroup(this);

			_label = window.AddLabel($"Source {index}: Stopped");

			window.FinishGroup();

			source.OnInstanceChanged += newInstance => _newInstance = newInstance;
		}

		protected override void Update()
		{
			if (_instance == _newInstance)
				return;

			Clear();

			if (_newInstance is null)
				Add(_label);
			else
			{
				_window.SelectGroup(this);
				_window.Add(new AudioInstanceInspector(_window, _newInstance, $"Source {_index}: Playing"));
				_window.FinishGroup();
			}

			_instance = _newInstance;
		}
	}

	class ByteSourceInspector : FlexContainer
	{
		private readonly GUIWindow _window;
		private readonly int _index;
		private readonly GUILabel _label;
		private IAudioInstance? _instance;
		private IAudioInstance? _newInstance;

		public ByteSourceInspector(GUIWindow window, int index, ALByteSource source)
		{
			_window = window;
			_index = index;

			Direction = FlexDirection.Vertical;
			RelativeSizeAxes = Axes.X;
			AutoSizeAxes = Axes.Y;

			window.SelectGroup(this);

			_label = window.AddLabel($"Source {index}: Stopped");

			window.FinishGroup();

			source.OnInstanceChanged += newInstance => _newInstance = newInstance;
		}

		protected override void Update()
		{
			if (_instance == _newInstance)
				return;

			Clear();

			if (_newInstance is null)
				Add(_label);
			else
			{
				_window.SelectGroup(this);
				_window.Add(new AudioInstanceInspector(_window, _newInstance, $"Source {_index}: Playing"));
				_window.FinishGroup();
			}

			_instance = _newInstance;
		}
	}

	class AudioInstanceInspector : FlexContainer
	{
		public AudioInstanceInspector(GUIWindow window, IAudioInstance instance, string title)
		{
			RelativeSizeAxes = Axes.X;
			AutoSizeAxes = Axes.Y;
			Direction = FlexDirection.Vertical;
			Spacing = new(0, 4);

			window.AddGroup(title);

			var durationText = TextUtils.FormatTimeCodeFromSeconds(instance.Duration);

			window.AddLabel($"Looping: {instance.Looping}");
			window.AddLabel($"Duration: {durationText}");
			window.AddPropertyLabel("State", instance.CreateProxy<AudioInstanceState>("State"));

			var gainSlider = window.AddSliderFloat("Gain", 0, 1, instance.Gain, "0.00");
			gainSlider.OnValueChanged(instance.SetGain);

			var timestampSlider = window.AddPropertySliderFloat("Timestamp",
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

			window.FinishGroup();
		}
	}
}
