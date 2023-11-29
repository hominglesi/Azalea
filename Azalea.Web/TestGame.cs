using Azalea.Graphics;
using Azalea.Graphics.Shapes;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Utils;
using System.Numerics;
using System.Timers;

namespace Azalea.Web;

public class TestGame : AzaleaGame
{
	private Timer _timer;
	private Sprite _azalea;

	protected override void OnInitialize()
	{
		Host.Window.Title = "Ide gas";
		Host.Window.Title += " a ide i plin";

		Host.Renderer.ClearColor = Color.Red;

		_timer = new Timer(1000 / 2)
		{
			AutoReset = true,
			Enabled = true
		};
		_timer.Elapsed += (_, _) => { Host.Renderer.ClearColor = Rng.Color(); };

		Add(new Box() { Size = new Vector2(200, 100), ColorInfo = Color.Aqua });
		Add(new Box() { Position = new Vector2(100, 400), Size = new Vector2(50, 100), ColorInfo = Color.Black });
		Add(_azalea = new Sprite() { Texture = Assets.GetTexture("azalea-icon.png"), Origin = Anchor.Center });
	}

	protected override void Update()
	{
		_azalea.Position = Input.MousePosition;
	}
}
