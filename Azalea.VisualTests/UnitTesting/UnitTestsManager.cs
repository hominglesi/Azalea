using Azalea.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestsManager
{
	public List<UnitTestSuite> UnitTestSuites { get; init; } = new();

	public UnitTestSuite SelectedSuite => UnitTestSuites[_selectedSuiteIndex];
	private int _selectedSuiteIndex;

	public UnitTest SelectedUnitTest => SelectedSuite.Tests[_selectedUnitTestIndex];
	private int _selectedUnitTestIndex;

	public UnitTestsManager()
	{
		var unitTests = ReflectionUtils.GetAllChildrenOf(typeof(UnitTestSuite)).Where(x => x.IsAbstract == false);
		foreach (var testType in unitTests)
		{
			var test = ReflectionUtils.InstantiateType<UnitTestSuite>(testType);
			if (test.Tests.Count < 1)
				continue;

			UnitTestSuites.Add(test!);
		}

		_selectedSuiteIndex = 0;
		_selectedUnitTestIndex = 0;
	}

	public void SelectNextSuite()
	{
		_selectedSuiteIndex++;
		if (_selectedSuiteIndex >= UnitTestSuites.Count)
			_selectedSuiteIndex = 0;

		_selectedUnitTestIndex = 0;
	}

	public void SelectNextUnitTest()
	{
		_selectedUnitTestIndex++;
		if (_selectedUnitTestIndex >= SelectedSuite.Tests.Count)
			_selectedUnitTestIndex = 0;
	}

	public IEnumerable<UnitTest> GetAllTests()
	{
		foreach (var suite in UnitTestSuites)
			foreach (var test in suite.Tests)
				yield return test;
	}
}
