using Azalea.Design.Containers;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform;
using System;

namespace Azalea.VisualTests;
public class FrameRateTest : TestScene
{
	private SpriteText _elapsedTimeText;
	private SpriteText _deltaTimeText;
	private SpriteText _fixedDeltaTimeText;

	public FrameRateTest()
	{
		Add(new FlexContainer()
		{
			RelativeSizeAxes = Graphics.Axes.Both,
			Direction = FlexDirection.Vertical,
			Children = [
				_elapsedTimeText = createText(),
				_deltaTimeText = createText(),
				_fixedDeltaTimeText = createText()
			]
		});
	}

	private float _deltaTimeCounter;

	private DateTime? _startTime;
	protected override void Update()
	{
		_startTime ??= Time.GetCurrentPreciseTime();

		_elapsedTimeText.Text = (Time.GetPreciseMilisecondsSince(_startTime.Value) / 1000).ToString();

		_deltaTimeCounter += Time.DeltaTime;
		_deltaTimeText.Text = _deltaTimeCounter.ToString();
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Keys.Space)
			Window.VSync = !Window.VSync;

		return base.OnKeyDown(e);
	}

	private float _fixedDeltaTimeCounter;
	protected override void FixedUpdate()
	{
		_fixedDeltaTimeCounter += 1 / (float)60;
		_fixedDeltaTimeText.Text = _fixedDeltaTimeCounter.ToString();
	}

	private static SpriteText createText()
		=> new()
		{
			Font = FontUsage.Default.With(size: 100)
		};

}
