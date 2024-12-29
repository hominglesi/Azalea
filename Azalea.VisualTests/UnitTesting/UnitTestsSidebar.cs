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
public class UnitTestsSidebar : Composition
{
	private readonly static float __headerHeight = 65;
	private readonly static float __suiteTitleHeight = 35;

	private readonly static float __stepHeight = 35;
	private readonly static float __stepCheckboxPadding = 5;
	private readonly static Color __stepBackgroundColor = new(252, 191, 73);
	private readonly static Color __stepDoneBackgroundColor = new(247, 127, 0);
	private readonly static Color __stepTextColor = new(0, 48, 73);
	private readonly static Color __stepCheckboxPassed = Palette.Green;
	private readonly static Color __stepCheckboxFailed = new(214, 40, 40);

	private FlexContainer _headerContainer;
	private SpriteText _suiteTitle;
	private FlexContainer _stepContainer;

	public UnitTestsSidebar()
	{
		BackgroundColor = new Color(0, 48, 73);

		Add(_headerContainer = new FlexContainer()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, __headerHeight),
			Justification = FlexJustification.Center,
			Alignment = FlexAlignment.Center,
			Wrapping = FlexWrapping.Wrap
		});

		Add(_suiteTitle = new SpriteText()
		{
			RelativePositionAxes = Axes.X,
			Position = new(0.5f, __headerHeight + (__suiteTitleHeight / 3)),
			Origin = Anchor.Center,
			Text = "Placeholder"
		});

		Add(new ScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Position = new(0, __headerHeight + __suiteTitleHeight),
			NegativeSize = new(0, __headerHeight + __suiteTitleHeight),
			Child = _stepContainer = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Vertical
			}
		});
	}

	public void AddHeaderButton(string iconName, Action clickAction)
	{
		var button = new HeaderButton(iconName);
		button.Click += _ => clickAction.Invoke();
		_headerContainer.Add(button);
	}

	private int _nextStep = 0;
	private List<TestStep> _steps = new();
	private List<TestStepButton> _stepButtons = new();

	public void DisplaySuite(UnitTestSuite suite)
	{
		_suiteTitle.Text = suite.DisplayName;
	}

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

		RunNextStepWithResult();
	}

	public bool RunNextStepWithResult()
	{
		var testResult = true;
		var step = _steps[_nextStep];
		var stepIndex = _nextStep;
		var stepButton = _stepButtons[stepIndex];

		if (step is TestStepOperation operation)
			operation.Action.Invoke();
		else if (step is TestStepResult result)
		{
			testResult = result.Action.Invoke();
			((TestStepResultButton)stepButton).SetResult(testResult);
		}

		stepButton.MarkAsDone();
		_nextStep++;
		return testResult;
	}

	public void RunAllSteps()
	{
		while (_nextStep < _steps.Count)
			RunNextStep();
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
