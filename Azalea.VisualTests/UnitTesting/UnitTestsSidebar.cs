using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using System;
using System.Collections.Generic;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestsSidebar : ContentContainer
{
	private readonly static float __headerHeight = 70;

	private readonly static float __stepHeight = 50;
	private readonly static float __stepCheckboxPadding = 10;
	private readonly static Color __stepBackgroundColor = new(252, 191, 73);
	private readonly static Color __stepDoneBackgroundColor = new(247, 127, 0);
	private readonly static Color __stepTextColor = new(0, 48, 73);
	private readonly static Color __stepCheckboxPassed = Palette.Green;
	private readonly static Color __stepCheckboxFailed = new(214, 40, 40);

	public event Action? RunStepPressed;
	public event Action? RunAllStepsPressed;
	public event Action? ResetPressed;

	private HeaderButton _runStepButton;
	private HeaderButton _runAllStepsButton;
	private HeaderButton _resetButton;

	private FlexContainer _headerContainer;
	private FlexContainer _stepContainer;

	public UnitTestsSidebar()
	{
		BackgroundColor = new Color(0, 48, 73);
		ContentComposition.Y = __headerHeight;

		AddInternal(_headerContainer = new FlexContainer()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, __headerHeight),
			Justification = FlexJustification.Center,
			Alignment = FlexAlignment.Center,
			Wrapping = FlexWrapping.Wrap,
			Children = new[]
			{
				_runStepButton = new HeaderButton("runStep"),
				_runAllStepsButton = new HeaderButton("runAllSteps"),
				_resetButton = new HeaderButton("reset")
			}
		});

		_runStepButton.Click += (_) => RunStepPressed?.Invoke();
		_runAllStepsButton.Click += (_) => RunAllStepsPressed?.Invoke();
		_resetButton.Click += (_) => ResetPressed?.Invoke();

		Add(new ScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Child = _stepContainer = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Vertical
			}
		});
	}

	private int _nextStep = 0;
	private List<TestStep> _steps = new();
	private List<TestStepButton> _stepButtons = new();

	public void AddSteps(List<TestStep> steps)
	{
		var lastStep = _steps.Count;

		for (int i = 0; i < steps.Count; i++)
		{
			var step = steps[i];
			var stepIndex = lastStep + i;
			TestStepButton? button = null;

			if (step is TestStepOperation)
				button = new TestStepOperationButton(step.Name);
			else if (step is TestStepResult)
				button = new TestStepResultButton(step.Name);

			if (button is null)
				throw new NotImplementedException($"Test Step not implemented");

			_stepContainer.Add(button);
			_stepButtons.Add(button);
		}

		_steps.AddRange(steps);
	}

	public void ClearSteps()
	{
		_steps.Clear();
		_stepButtons.Clear();
		_stepContainer.Clear();
		_nextStep = 0;
	}

	public void RunNextStep()
	{
		if (_nextStep >= _steps.Count)
			return;

		var step = _steps[_nextStep];
		var stepIndex = _nextStep;
		var stepButton = _stepButtons[stepIndex];

		if (step is TestStepOperation operation)
			operation.Action.Invoke();
		else if (step is TestStepResult result)
		{
			var testResult = result.Action.Invoke();
			((TestStepResultButton)stepButton).SetResult(testResult);
		}

		stepButton.MarkAsDone();
		_nextStep++;
	}

	public void RunAllSteps()
	{
		while (_nextStep < _steps.Count)
			RunNextStep();
	}

	protected override void UpdateContentLayout()
	{
		ContentComposition.Height = DrawHeight - __headerHeight;
		ContentComposition.Width = DrawWidth;
	}

	private class HeaderButton : Button
	{
		private readonly Sprite _backgroundSprite;
		private readonly Sprite _iconSprite;

		public HeaderButton(string iconName)
		{
			Margin = new(15, 5, 15, 5);
			Size = new(__headerHeight - 30);

			Add(_backgroundSprite = new Sprite()
			{
				RelativeSizeAxes = Axes.Both,
				Texture = Assets.GetTexture("Textures/Icons/background.png"),
				Color = new Color(234, 226, 183)
			});

			Add(_iconSprite = new Sprite()
			{
				RelativeSizeAxes = Axes.Both,
				Size = new(0.65f),
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
				Texture = Assets.GetTexture($"Textures/Icons/{iconName}.png"),
				Color = new Color(214, 40, 40)
			});
		}

		protected override bool OnHover(HoverEvent e)
		{
			_backgroundSprite.Color = new Color(214, 40, 40);
			_iconSprite.Color = new Color(234, 226, 183);

			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			_backgroundSprite.Color = new Color(234, 226, 183);
			_iconSprite.Color = new Color(214, 40, 40);
		}
	}

	private class TestStepButton : Composition
	{
		internal readonly Box Background;
		internal readonly SpriteText Text;

		public TestStepButton(string text)
		{
			RelativeSizeAxes = Axes.X;
			Size = new(1, __stepHeight);

			Add(Background = new Box()
			{
				RelativeSizeAxes = Axes.Both,
				Color = __stepBackgroundColor
			});

			Add(Text = new SpriteText()
			{
				Text = text,
				Color = __stepTextColor,
				Anchor = Anchor.CenterLeft,
				Origin = Anchor.CenterLeft,
				Position = new(5, 0)
			});
		}

		public void MarkAsDone()
		{
			Background.Color = __stepDoneBackgroundColor;
		}
	}

	private class TestStepOperationButton : TestStepButton
	{
		public TestStepOperationButton(string text)
			: base(text) { }
	}

	private class TestStepResultButton : TestStepButton
	{
		private readonly Sprite _checkbox;

		public TestStepResultButton(string text)
			: base(text)
		{
			Add(_checkbox = new Sprite()
			{
				Position = new(-__stepCheckboxPadding, 0),
				Origin = Anchor.CenterRight,
				Anchor = Anchor.CenterRight,
				Size = new(__stepHeight - (__stepCheckboxPadding * 2)),
				Texture = Assets.GetTexture("Textures/Icons/checkbox-empty.png")
			});
		}

		public void SetResult(bool result)
		{
			_checkbox.Texture = Assets.GetTexture($"Textures/Icons/checkbox-{(result ? "passed" : "failed")}.png");
			_checkbox.Color = result ? __stepCheckboxPassed : __stepCheckboxFailed;
		}
	}
}
