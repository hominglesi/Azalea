using System;

namespace Azalea.Amends;

public abstract class Amend<T>(T target, Action<T>? action) : IAmend
{
	protected T Target = target;
	protected Action<T>? Action = action;

	public bool IsFinished { get; protected set; }
	public bool HasStarted { get; protected set; }

	public virtual void Start()
	{
		HasStarted = true;
	}

	public virtual void Perform()
	{
		Action?.Invoke(Target);
	}

	public virtual void Finish()
	{
		IsFinished = true;
	}

	public abstract void Update(float deltaTime);
}
