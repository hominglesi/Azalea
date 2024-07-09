using Azalea.Utils;
using Azalea.VisualTests.UnitTesting;
using System.Collections.Generic;

namespace Azalea.VisualTests;
public abstract class UnitTestSuite
{
	public string DisplayName { get; set; } = "";
	public List<UnitTest> Tests { get; init; } = new();

	public UnitTestSuite()
	{
		foreach (var type in GetType().GetNestedTypes())
		{
			if (type.IsAssignableTo(typeof(UnitTest)) == false) continue;

			var test = ReflectionUtils.InstantiateType<UnitTest>(type);
			Tests.Add(test);
		}
	}
}
