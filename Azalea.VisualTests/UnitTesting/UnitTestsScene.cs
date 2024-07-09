using Azalea.Design.Containers;
using Azalea.Graphics;

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

		selectUnitTest(_manager.SelectedSuite.Tests[0]);
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
}
