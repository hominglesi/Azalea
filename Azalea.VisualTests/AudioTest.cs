using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Sounds;

namespace Azalea.VisualTests;
internal class AudioTest : TestScene
{
	private Sound _goodkid;
	private Sound _harbor;
	private SoundByte _hitnormal;

	private IAudioInstance _instance;
	private Button _hitnormalButton;

	public AudioTest()
	{
		_goodkid = Assets.GetSound("Audio/goodkid.wav");
		_hitnormal = Assets.GetSoundByte("Audio/hitnormal.wav");
		_harbor = Assets.FileSystemStore.GetSound(@"E:\Music\Alohaii\Virtual Paradise\\10 - Harbor (feat. Kaneko Lumi).flac");

		AddRange(new GameObject[] {
			CreateFullscreenVerticalFlex(new GameObject[]
			{
				CreateActionButton(
					"'goodkid.wav' at 0.1f",
					() => _instance = Audio.PlayAudio(_goodkid, 0.1f)),
				CreateActionButton(
					"'Harbor (feat. Kaneko Lumi)' at 0.1f",
					() => _instance = Audio.PlayAudio(_harbor, 0.1f)),
				_hitnormalButton = CreateActionButton(
					"'hitnormal.wav' at 0.1f",
					() => _instance = Audio.PlayAudioByte(_hitnormal, 0.1f)),
				/*
				CreateActionButton(
					"Play looping 'hitnormal.wav' at 0.1f",
					() => _instance = Audio.PlayLegacyAudio(_hitnormal, 0.1f, true)),
				CreateActionButton(
					"Set _instance gain to 0.1f",
					() => { if (_instance is not null) _instance.Gain = 0.1f; }),
				CreateActionButton(
					"Set _instance gain to 0.6f",
					() => { if (_instance is not null) _instance.Gain = 0.6f; }),
				CreateActionButton(
					"Set master volume to 1f",
					() => Audio.MasterVolume = 1f),
				CreateActionButton(
					"Set  master volume to 0.5f",
					() => Audio.MasterVolume = 0.5f),
				CreateActionButton(
					"Stop _instance",
					() => { _instance?.Stop(); }),
				CreateActionButton(
					"Garbage collect",
					() => GC.Collect())
				*/
			}),
			/*
			CreateObservedContainer(new GameObject[]
			{
				CreateObservedValue("_instance Playing",
					() => _instance is not null ? _instance.Playing : false),
				CreateObservedValue("_instance Gain",
					() => _instance is not null && _instance.Playing ? _instance.Gain : -1)
			})*/
		});

		Add(new BasicWindow()
		{
			X = 1000,
			Y = 50,
			Child = new AudioManagerDetailsView(Audio.Instance)
		});
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

			AddRange([
				new SpriteText(){
					Text = "Total audio byte channels: " + _audioManager.AudioByteChannels,
					Color = Palette.Gray
				},
				new SpriteText(){
					Text = "Total internal audio byte channels: " + _audioManager.AudioByteInternalChannels,
					Color = Palette.Gray
				},
			]);
		}

		class IAudioSourceDetailsView : FlexContainer
		{
			private readonly IAudioSource _audioSource;

			private readonly SpriteText _stateDisplay;
			private readonly BasicSlider _volumeSlider;

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
					}
				]);

				_volumeSlider.Body.Color = Palette.Black;
				_volumeSlider.OnValueChanged += value => _audioSource.Volume = value;
			}

			private void onStateUpdated(AudioSourceState state)
			{
				_stateDisplay.Text = "State: " + _audioSource.State;
				_volumeSlider.Value = _audioSource.Volume;
			}
		}
	}
}
