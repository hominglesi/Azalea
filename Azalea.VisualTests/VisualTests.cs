//using Azalea.Audios;
using Azalea.Graphics.Colors;
using Azalea.IO.Resources;

namespace Azalea.VisualTests;

public class VisualTests : AzaleaGame
{
	protected override void OnInitialize()
	{
		Assets.AddToMainStore(new NamespacedResourceStore(new AssemblyResourceStore(typeof(VisualTests).Assembly), "Resources"));

		Host.Renderer.ClearColor = Palette.Flowers.Azalea;

		//Add(new DefaultUserInputTest());
		//Add(new TestingTestScene());
		//Add(new FlexTest());
		//Add(new TextContainerTest());
		//Add(new BilliardTest());
		//Add(new PhysicsTest());
		//Add(new TriggerTest());
		//Add(new SliderTests());
		//Add(new AutoSizeTest());
		//Add(new InputTest());
		//Add(new PanningTest());
		//Add(new IWindowTest());
		//Add(new AudioTest());
		//Add(new BreakoutTest());
		//Add(new BoundingBoxTreeTest());
		Add(new CameraTest());
	}

	protected override void Update()
	{
		/*
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

		if (Input.GetKey(Keys.L).Down) Audio.Play(_sound2);*/
	}
}
