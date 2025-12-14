using Azalea.Utils;
using System.Collections.Generic;

namespace Azalea.VisualTests.UnitTesting;
public abstract class UnitTestSuite : UnitTestBase
{
	public List<UnitTest> Tests { get; init; } = new();

	public UnitTestSuite()
	{
		foreach (var type in GetType().GetNestedTypes())
		{
			if (type.IsAssignableTo(typeof(UnitTest)) == false) continue;

			var test = ReflectionUtils.InstantiateType<UnitTest>(type);
			test.Suite = this;
			Tests.Add(test);
		}
	}
}
