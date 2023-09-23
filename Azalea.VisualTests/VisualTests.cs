using Azalea.Audios;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using System;

namespace Azalea.VisualTests;

public class VisualTests : AzaleaGame
{
	private Sound _sound;
	private Sound _sound2;
	private AudioInstance? _instance;

	protected override void OnInitialize()
	{
		Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(VisualTests).Assembly), "Resources"));

		Host.Renderer.ClearColor = Palette.Flowers.Azalea;
		Host.Window.Resizable = true;

		//Add(new DefaultUserInputTest());
		//Add(new TestingTestScene());
		//Add(new FlexTest());
		Add(new InputTest());

		_sound = Assets.GetSound("Audio/paramore.wav");
		_sound2 = Assets.GetSound("Audio/audio.wav");

		var color = Palette.Lime;
		color.Luminance -= 0.45f;
		Console.WriteLine(color);
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Escape).Down) Host.Window.Close();

		if (Input.GetKey(Keys.K).Down)
		{
			if (_instance is not null)
			{
				_instance.Stop();
				_instance = null;
			}
			else
			{
				_instance = Audio.Play(_sound);
			}
		}

		if (Input.GetKey(Keys.L).Down) Audio.Play(_sound2);
	}
}
