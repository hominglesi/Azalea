using Azalea.Graphics;
using System;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestsScene : TestScene
{
	private static float __menuBarHeight = 30;
	private static float __sidebarWidth = 340;

	private UnitTestsManager _manager;
	private UnitTestMenuBar _menuBar;
	private UnitTestsSidebar _sidebar;
	private UnitTestContainer _testContainer;

	public UnitTestsScene()
	{
		AzaleaGame.Main.Host.Renderer.ClearColor = new(255, 248, 211);

		_manager = new UnitTestsManager();

		Add(_menuBar = new UnitTestMenuBar()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, __menuBarHeight)
		});

		_menuBar.AddMenuButton("Next Suite", selectNextTestSuite);
		_menuBar.AddMenuButton("Next Test", selectNextUnitTest);
		_menuBar.AddMenuButton("Run All", runAllTests);

		Add(_sidebar = new UnitTestsSidebar()
		{
			Position = new(0, 30),
			RelativeSizeAxes = Axes.Y,
			Size = new(__sidebarWidth, 1),
			NegativeSize = new(0, __menuBarHeight)
		});

		_sidebar.AddHeaderButton("runStep", _sidebar.RunNextStep);
		_sidebar.AddHeaderButton("runAllSteps", _sidebar.RunAllSteps);
		_sidebar.AddHeaderButton("reset", resetUnitTest);

		Add(_testContainer = new UnitTestContainer()
		{
			Position = new(__sidebarWidth, __menuBarHeight),
			RelativeSizeAxes = Axes.Both,
			NegativeSize = new(__sidebarWidth, __menuBarHeight)
		});

		displayUnitTest(_manager.SelectedUnitTest);
	}

	private UnitTest? _displayedUnitTest;
	private void displayUnitTest(UnitTest unitTest)
	{
		_displayedUnitTest?.TearDown(_testContainer);
		_sidebar.ClearSteps();

		_displayedUnitTest = unitTest;
		if (_displayedUnitTest is null) return;

		_displayedUnitTest.Setup(_testContainer);
		_sidebar.DisplaySuite(_displayedUnitTest.Suite!);
		_sidebar.AddSteps(_displayedUnitTest.Steps);
		_displayedUnitTest = unitTest;
	}

	private void selectNextTestSuite()
	{
		_manager.SelectNextSuite();
		displayUnitTest(_manager.SelectedUnitTest);
	}

	private void selectNextUnitTest()
	{
		_manager.SelectNextUnitTest();
		displayUnitTest(_manager.SelectedUnitTest);
	}

	private void resetUnitTest()
	{
		if (_displayedUnitTest is null)
			return;

		_displayedUnitTest.TearDown(_testContainer);
		_sidebar.ClearSteps();

		_displayedUnitTest.Setup(_testContainer);
		_sidebar.AddSteps(_displayedUnitTest.Steps);
	}

	private void runAllTests()
	{
		foreach (var test in _manager.GetAllTests())
		{
			displayUnitTest(test);
			for (int i = 0; i < test.Steps.Count; i++)
			{
				var result = _sidebar.RunNextStepWithResult();

				if (result == false)
					Console.WriteLine($"FAILED: {test.Suite!.DisplayName}/{test.DisplayName}/Step {i + 1}: {test.Steps[i].Name}");
			}
		}
	}
}
