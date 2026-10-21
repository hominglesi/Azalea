using Azalea.Platform;
using System;
using System.Collections.Generic;

namespace Azalea.VisualTests.UnitTesting;

public abstract class UnitTest : UnitTestBase
{
	public UnitTestSuite? Suite { get; set; }
	public UnitTestContainer? TestContainer { get; set; }
	public List<TestStep> Steps { get; init; } = [];

	public Application App => TestContainer!.App;

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

public class TestStep(string name)
{
	public readonly string Name = name;
}

public class TestStepOperation(string name, Action action) : TestStep(name)
{
	public readonly Action Action = action;
}

public delegate bool TestStepResultDelegate();

public class TestStepResult(string name, TestStepResultDelegate action) : TestStep(name)
{

	public readonly TestStepResultDelegate Action = action;
}
