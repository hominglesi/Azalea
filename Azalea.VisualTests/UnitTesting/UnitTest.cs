using System;
using System.Collections.Generic;

namespace Azalea.VisualTests.UnitTesting;
public abstract class UnitTest
{
	public string DisplayName { get; init; }
	public UnitTestSuite? Suite { get; set; }
	public UnitTestContainer? TestContainer { get; set; }
	public List<TestStep> Steps { get; init; } = new();

	public UnitTest()
	{
		DisplayName = VisualTestUtils.GetTestDisplayName(GetType());
	}

	internal void AddOperation(string name, Action action)
		=> Steps.Add(new TestStepOperation(name, action));

	internal void AddResult(string name, TestStepResultDelegate action)
		=> Steps.Add(new TestStepResult(name, action));

	public virtual void Setup(UnitTestContainer scene)
	{
		TestContainer = scene;
	}
	public virtual void TearDown(UnitTestContainer scene)
	{
		TestContainer = null;
	}
}

public class TestStep
{
	public readonly string Name;

	public TestStep(string name)
	{
		Name = name;
	}
}

public class TestStepOperation : TestStep
{
	public readonly Action Action;

	public TestStepOperation(string name, Action action)
		: base(name)
	{
		Action = action;
	}
}

public delegate bool TestStepResultDelegate();

public class TestStepResult : TestStep
{

	public readonly TestStepResultDelegate Action;

	public TestStepResult(string name, TestStepResultDelegate action)
		: base(name)
	{
		Action = action;
	}
}
