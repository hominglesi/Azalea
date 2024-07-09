using Azalea.Design.Containers;
using System;
using System.Collections.Generic;

namespace Azalea.VisualTests.UnitTesting;
public abstract class UnitTest
{
	public string DisplayName { get; set; } = "";

	public List<TestStep> Steps { get; init; } = new();

	internal void AddOperation(string name, Action action)
		=> Steps.Add(new TestStepOperation(name, action));

	internal void AddResult(string name, TestStepResultDelegate action)
		=> Steps.Add(new TestStepResult(name, action));

	public virtual void Setup(Composition scene) { }
	public virtual void TearDown(Composition scene) { }
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
