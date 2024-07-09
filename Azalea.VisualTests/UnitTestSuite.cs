using Azalea.Utils;
using Azalea.VisualTests.UnitTesting;
using System.Collections.Generic;

namespace Azalea.VisualTests;
public abstract class UnitTestSuite
{
	public string DisplayName { get; init; }
	public List<UnitTest> Tests { get; init; } = new();

	public UnitTestSuite()
	{
		DisplayName = VisualTestUtils.GetTestDisplayName(GetType());

		foreach (var type in GetType().GetNestedTypes())
		{
			if (type.IsAssignableTo(typeof(UnitTest)) == false) continue;

			var test = ReflectionUtils.InstantiateType<UnitTest>(type);
			test.Suite = this;
			Tests.Add(test);
		}
	}
}
