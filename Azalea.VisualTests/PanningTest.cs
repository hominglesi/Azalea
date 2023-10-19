using Azalea.Design.Containers;
using Azalea.Graphics.UserInterface;
using Azalea.Inputs;
using Azalea.Platform;
using System.Numerics;

namespace Azalea.VisualTests;
public class PanningTest : TestScene
{
	private PannableContainer _container;

	public PanningTest()
	{
		Add(_container = new PannableContainer()
		{
			RelativeSizeAxes = Graphics.Axes.Both,
			Origin = Graphics.Anchor.Center,
			Children = new[]
			{
				new BasicButton()
				{
					Position = new(900, 900),
					Text = "Hey guys its me video game dunkey",
					Rotation = 40,
					Width = 500
				},
				new BasicButton()
				{
					Position = new(500, 600),
					Text = "Hey guys its me video game dunkey",
					Width = 300
				},
				new BasicButton()
				{
					Position = new(1400, 700),
					Text = "Hey guys its me video game dunkey",
					Width = 450,
					Height = 24,
					Rotation = -150
				},
				new BasicButton()
				{
					Position = new(1400, 950),
					Text = "Hey guys its me video game dunkey",
					Width = 320,
					Height = 100
				}
			}
		});
	}

	protected override void Update()
	{
		var panSpeed = 0.5f * Time.DeltaTimeMs;
		if (Input.GetKey(Keys.Up).Pressed) _container.MoveCameraBy(new Vector2(0, panSpeed));
		if (Input.GetKey(Keys.Right).Pressed) _container.MoveCameraBy(new Vector2(-panSpeed, 0));
		if (Input.GetKey(Keys.Down).Pressed) _container.MoveCameraBy(new Vector2(0, -panSpeed));
		if (Input.GetKey(Keys.Left).Pressed) _container.MoveCameraBy(new Vector2(panSpeed, 0));

		if (Input.GetKey(Keys.KeypadPlus).Down) _container.ZoomCameraBy(new Vector2(0.15f, 0.15f));
		if (Input.GetKey(Keys.KeypadMinus).Down) _container.ZoomCameraBy(new Vector2(-0.15f, -0.15f));
	}
}
