using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.IO.Configs;
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
		Renderer.ClearColor = new(255, 248, 211);

		_manager = new UnitTestsManager();

		Add(_menuBar = new UnitTestMenuBar()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, __menuBarHeight)
		});

		_menuBar.AddMenuButton("Run All", runAllTests);

		Add(_sidebar = new UnitTestsSidebar()
		{
			Position = new(0, 30),
			RelativeSizeAxes = Axes.Y,
			Size = new(__sidebarWidth, 1),
			NegativeSize = new(0, __menuBarHeight)
		});

		var selectedSuite = Config.GetInt("currentUnitTestSuite") ?? 0;
		var selectedTest = Config.GetInt("currentUnitTest") ?? 0;

		populateSuiteSelectMenu();
		populateTestSelectMenu(selectedSuite);

		_sidebar.SuiteSelectMenu.SelectOption(selectedSuite);
		_sidebar.TestSelectMenu.SelectOption(selectedTest);
		_sidebar.SuiteSelectMenu.OnSelectedChanged += val => selectSuite(int.Parse(val!));
		_sidebar.TestSelectMenu.OnSelectedChanged += val => selectUnitTest(int.Parse(val!));

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

	private void populateSuiteSelectMenu()
	{
		_sidebar.SuiteSelectMenu.ClearOptions();

		for (int i = 0; i < _manager.UnitTestSuites.Count; i++)
		{
			var suite = _manager.UnitTestSuites[i].DisplayName;
			_sidebar.SuiteSelectMenu.AddOption(suite, i.ToString());
		}
	}

	private void populateTestSelectMenu(int suite)
	{
		_sidebar.TestSelectMenu.ClearOptions();

		for (int i = 0; i < _manager.UnitTestSuites[suite].Tests.Count; i++)
		{
			var unitTest = _manager.UnitTestSuites[suite].Tests[i].DisplayName;
			_sidebar.TestSelectMenu.AddOption(unitTest, i.ToString());
		}
	}

	private UnitTest? _displayedUnitTest;
	private void displayUnitTest(UnitTest unitTest)
	{
		_displayedUnitTest?.TearDown(_testContainer);
		_sidebar.ClearSteps();

		_displayedUnitTest = unitTest;
		if (_displayedUnitTest is null) return;

		_displayedUnitTest.Setup(_testContainer);
		_sidebar.AddSteps(_displayedUnitTest.Steps);
		_displayedUnitTest = unitTest;
	}

	private void selectSuite(int index)
	{
		_manager.SelectSuite(index);

		populateTestSelectMenu(index);
		_sidebar.TestSelectMenu.SelectOption(0);

		Config.Set("currentUnitTestSuite", index);
		Config.Set("currentUnitTest", 0);

		displayUnitTest(_manager.SelectedUnitTest);
	}

	private void selectUnitTest(int index)
	{
		_manager.SelectUnitTest(index);

		Config.Set("currentUnitTest", index);

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
