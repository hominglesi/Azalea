using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.IO.Stores;
using Azalea.Platform.Silk;

namespace Azalea.VisualTests;

public class VisualTests : AzaleaGame
{
	private SilkAudio.AudioInstance _audio;

	protected override void OnInitialize()
	{
		Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(VisualTests).Assembly), "Resources"));

		Host.Renderer.ClearColor = Color.Azalea;
		Host.Window.Resizable = true;

		//Add(new DefaultUserInputTest());
		Add(new TestingTestScene());
		//Add(new FlexTest());

		_audio = SilkAudio.Instance.CreateInstance("Audio/audio.wav");
		SilkAudio.Instance?.PlaySound(_audio);
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Escape).Down) Host.Window.Close();

		if (Input.GetKey(Keys.K).Down) SilkAudio.Instance?.PlaySound(_audio);
	}
}
