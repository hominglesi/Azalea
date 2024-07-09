using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestsManager
{
	public List<UnitTest> UnitTests { get; init; } = new();

	public UnitTestsManager()
	{
		var unitTests = ReflectionUtils.GetAllChildrenOf(typeof(UnitTest)).Where(x => x.IsAbstract == false).Select(x => x.FullName!).ToList()!;
		foreach (var testName in unitTests)
		{
			var test = Activator.CreateInstance(Assembly.GetAssembly(typeof(VisualTests))!.FullName!, testName!)!.Unwrap() as UnitTest;
			test!.DisplayName = VisualTestUtils.GetTestDisplayName(testName);
			UnitTests.Add(test!);
		}
	}
}
