using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Sounds;
using System;

namespace Azalea.VisualTests;
internal class AudioTest : TestScene
{
	private Sound _goodkid;
	private SoundByte _goodkidByte;

	private Sound _harbor;
	private Sound _navia;
	private SoundByte _hitnormal;

	private IAudioInstance _instance;

	public AudioTest()
	{
		_goodkid = Assets.GetSound("Audio/goodkid.wav");
		_goodkidByte = Assets.GetSoundByte("Audio/goodkidByte.wav");

		_harbor = Assets.FileSystemStore.GetSound(@"E:\Music\Alohaii\Virtual Paradise\\10 - Harbor (feat. Kaneko Lumi).flac");
		_hitnormal = Assets.GetSoundByte("Audio/hitnormal.wav");
		_navia = Assets.FileSystemStore.GetSound(@"E:\cut small.mp3");

		Add(
			CreateFullscreenVerticalFlex(new GameObject[]
			{
				CreateActionButton(
					"'goodkid.wav' at 0.1f",
					() => _instance = Audio.Play(_goodkid, 0.1f)),
				CreateActionButton(
					"'Harbor (feat. Kaneko Lumi)' at 0.1f",
					() => _instance = Audio.Play(_harbor, 0.1f)),
				CreateActionButton(
					"'Navia Song' at 0.1f",
					() => _instance = Audio.Play(_navia, 0.1f)),
				CreateActionButton(
					"'hitnormal.wav' at 0.1f",
					() => _instance = Audio.PlayByte(_hitnormal, 0.1f)),
				CreateActionButton(
					"'goodkid.wav' as Byte at 0.1f",
					() => _instance = Audio.PlayByte(_goodkidByte, 0.1f)),
				CreateActionButton(
					"Stress test audio playback",
					() => Add(new AudioRunner(_harbor))),
			})
		);

		Add(new BasicWindow()
		{
			X = 500,
			Y = 50,
			Width = 1100,
			Child = new AudioManagerDetailsView(Audio.Instance)
		});
	}

	class AudioRunner(Sound sound) : GameObject
	{
		private const int _runs = 100_000;

		private readonly Sound _sound = sound;
		private int _remainingRuns = _runs;
		private IAudioInstance? _instance;
		bool playing = false;

		protected override void Update()
		{
			if (_remainingRuns <= 0)
			{
				OnFinished?.Invoke();
				((Composition)Parent!).Remove(this);
				return;
			}

			if (playing == false)
			{
				_instance = Audio.Play(_sound);
				playing = true;
			}
			else
			{
				_instance!.Stop();
				_remainingRuns--;
				playing = false;
			}
		}

		public Action? OnFinished;
	}

	class AudioManagerDetailsView : FlexContainer
	{
		private readonly IAudioManager _audioManager;

		public AudioManagerDetailsView(IAudioManager audioManager)
		{
			_audioManager = audioManager;

			Y = 40;

			Direction = FlexDirection.Vertical;

			Add(new SpriteText()
			{
				Text = "Total audio channels: " + _audioManager.AudioChannels.Length,
				Color = Palette.Gray
			});

			foreach (var source in _audioManager.AudioChannels)
				Add(new IAudioSourceDetailsView(source));

			Add(new SpriteText()
			{
				Text = "Total audio byte channels: " + _audioManager.AudioByteChannels.Length,
				Color = Palette.Gray
			});

			foreach (var source in _audioManager.AudioByteChannels)
				Add(new IAudioSourceDetailsView(source));

			Add(new SpriteText()
			{
				Text = "Total internal audio byte channels: " + _audioManager.AudioByteInternalChannels.Length,
				Color = Palette.Gray
			});

			foreach (var source in _audioManager.AudioByteInternalChannels)
				Add(new IAudioSourceDetailsView(source));
		}

		class IAudioSourceDetailsView : FlexContainer
		{
			private readonly IAudioSource _audioSource;

			private readonly SpriteText _stateDisplay;
			private readonly SpriteText _durationDisplay;
			private readonly SpriteText _timestampDisplay;
			private readonly BasicSlider _volumeSlider;
			private readonly BasicSlider _seekSlider;
			private readonly Checkbox _loopingCheckbox;
			private readonly BasicSlider _pitchSlider;

			public IAudioSourceDetailsView(IAudioSource audioSource)
			{
				_audioSource = audioSource;
				_audioSource.StateUpdated += onStateUpdated;

				Size = new(1, 40);
				RelativeSizeAxes = Axes.X;
				Direction = FlexDirection.Horizontal;

				AddRange([
					_stateDisplay = new SpriteText()
					{
						Text = "State: " + _audioSource.State,
						Color = Palette.Gray
					},
					_durationDisplay = new SpriteText()
					{
						Text = "Duration: *:**",
						Color = Palette.Gray
					},
					new BasicButton()
					{
						Size = new(80, 40),
						Text = "Pause",
						ClickAction = _ => _audioSource.Pause()
					},
					new BasicButton()
					{
						Size = new(80, 40),
						Text = "Unpause",
						ClickAction = _ => _audioSource.Unpause()
					},
					new BasicButton()
					{
						Size = new(60, 40),
						Text = "Stop",
						ClickAction = _ => _audioSource.Stop()
					},
					_volumeSlider = new BasicSlider(){
						Size = new(100, 40),
						Value = 1
					},
					_seekSlider = new BasicSlider(){
						Size = new(100, 40)
					},
					new SpriteText()
					{
						Text = "Looping: ",
						Color = Palette.Gray
					},
					_loopingCheckbox = new Checkbox(),
					_pitchSlider = new BasicSlider(){
						Size = new(100, 40),
						Value = 0.5f
					},
					_timestampDisplay = new SpriteText()
					{
						Text = "Timestamp: *:**",
						Color = Palette.Gray
					},
				]);

				_volumeSlider.Body.Color = Palette.Black;
				_seekSlider.Body.Color = Palette.Black;
				_pitchSlider.Body.Color = Palette.Black;
				_volumeSlider.OnValueChanged += value => _audioSource.Volume = value;
				_seekSlider.OnValueSet += onSeekSet;
				_loopingCheckbox.Toggled += isChecked => _audioSource.Looping = isChecked;
				_pitchSlider.OnValueChanged += value => _audioSource.Pitch = 0.5f + value;
			}

			private void onStateUpdated(AudioSourceState state)
			{
				_stateDisplay.Text = "State: " + _audioSource.State;
				_volumeSlider.Value = _audioSource.Volume;

				if (_audioSource.CurrentInstance is not null)
				{
					var duration = _audioSource.CurrentInstance.TotalDuration;
					var minutes = (int)Math.Round(duration) / 60;
					var seconds = (int)Math.Round(duration % 60);
					_durationDisplay.Text = $"Duration: {minutes}:{seconds}";
				}
				else
					_durationDisplay.Text = $"Duration: *:**";
			}

			private void onSeekSet(float value)
			{
				if (_audioSource.CurrentInstance is null)
					return;

				var seconds = value * _audioSource.CurrentInstance.TotalDuration;

				_audioSource.Seek(seconds);
			}

			protected override void Update()
			{
				if (_audioSource.CurrentInstance is null)
					_timestampDisplay.Text = "Timestamp: *:**";
				else
				{
					var duration = _audioSource.CurrentInstance.CurrentTimestamp;
					var minutes = (int)Math.Round(duration) / 60;
					var seconds = (int)Math.Round(duration % 60);
					_timestampDisplay.Text = $"Timestamp: {minutes}:{seconds}";
					Console.WriteLine(_audioSource.CurrentInstance.CurrentTimestamp);
				}
			}
		}
	}
}
