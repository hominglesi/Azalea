using Azalea.Design.Containers;
using Azalea.Graphics;
using System;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestsScene : TestScene
{
	private UnitTestsManager _manager;
	private UnitTestsSidebar _sidebar;
	private Composition _testContainer;

	private UnitTest? _selectedTest;

	public UnitTestsScene()
	{
		AzaleaGame.Main.Host.Renderer.ClearColor = new(255, 248, 211);

		_manager = new UnitTestsManager();
		Add(_sidebar = new UnitTestsSidebar()
		{
			RelativeSizeAxes = Axes.Both,
			Size = new(0.25f, 1)
		});

		_sidebar.RunStepPressed += _sidebar.RunNextStep;
		_sidebar.RunAllStepsPressed += _sidebar.RunAllSteps;
		_sidebar.ResetPressed += resetUnitTest;


		Add(_testContainer = new Composition()
		{
			RelativePositionAxes = Axes.X,
			Position = new(0.25f, 0),
			RelativeSizeAxes = Axes.Both,
			Size = new(0.75f, 1)
		});

		selectUnitTest(_manager.UnitTests[0]);
	}

	private void selectUnitTest(UnitTest? unitTest)
	{
		_selectedTest?.TearDown(_testContainer);
		_sidebar.ClearSteps();

		_selectedTest = unitTest;
		if (_selectedTest is null) return;

		_selectedTest.Setup(_testContainer);
		_sidebar.AddSteps(_selectedTest.Steps);
	}

	private void resetUnitTest()
		=> selectUnitTest(_selectedTest);

	private void runAllTests()
	{
		foreach (var test in _manager.UnitTests)
		{
			test.Setup(_testContainer);

			foreach (var step in test.Steps)
			{
				if (step is TestStepOperation operation)
					operation.Action.Invoke();

				else if (step is TestStepResult result)
				{
					var testResult = result.Action.Invoke();
					if (testResult == false)
						Console.WriteLine($"Check in {test.DisplayName}: '{step.Name}' failed.");
				}
			}

			test.TearDown(_testContainer);

			Console.WriteLine($"All steps in {test.DisplayName} Test ran.");
		}

		resetUnitTest();
	}
}
