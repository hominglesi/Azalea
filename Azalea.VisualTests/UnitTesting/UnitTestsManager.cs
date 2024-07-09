using Azalea.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestsManager
{
	public List<UnitTestSuite> UnitTestSuites { get; init; } = new();

	public UnitTestSuite SelectedSuite { get; private set; }

	public UnitTestsManager()
	{
		var unitTests = ReflectionUtils.GetAllChildrenOf(typeof(UnitTestSuite)).Where(x => x.IsAbstract == false);
		foreach (var testType in unitTests)
		{
			var test = ReflectionUtils.InstantiateType<UnitTestSuite>(testType);
			test!.DisplayName = VisualTestUtils.GetTestDisplayName(testType.FullName!);
			UnitTestSuites.Add(test!);
		}

		SelectedSuite = UnitTestSuites[1];
	}
}
